using DIDAStorage;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using CHashing;
using System.Threading.Tasks;

namespace StorageNode
{
    class StorageNodeLogic 
    {
        public static int MAX_VERSIONS_STORED = 5;
        public static int CONSENSUS_TIMEOUT = 5000;

        private Dictionary<string, StorageNodeStruct> storageNodes = new Dictionary<string, StorageNodeStruct>();

        // if uivi cant write key during consensus
        private Dictionary<string, ConsensusKeyLock> consensusLock = new Dictionary<string, ConsensusKeyLock>();

        // Must be a queue instead of a list, in order to pop old values
        private Dictionary<string, List<DIDAStorage.DIDARecord>> storage = new Dictionary<string, List<DIDAStorage.DIDARecord>>();
        private ReplicaManager replicaManager;
        private string serverId;
        private int replicaId;
        private int gossipDelay;
        private Timer timer;
        private bool startedGossip = false;
        public StorageNodeLogic(string serverId, int replicaId, int gossipDelay)
        {
            this.serverId = serverId;
            this.replicaId = replicaId;
            this.gossipDelay = gossipDelay;
            this.replicaManager = new ReplicaManager(this.replicaId, MAX_VERSIONS_STORED);
            this.timer = new Timer();
            timer.Interval = this.gossipDelay;
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.AutoReset = true;
            timer.Enabled = false;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        public StatusReply Status()
        {
            lock (storage)
            {
                Console.WriteLine("This is my Status: " + replicaId);

                foreach (KeyValuePair<string, List<DIDAStorage.DIDARecord>> pair in storage)
                {
                    Console.WriteLine("Key: " + pair.Key);
                    foreach (DIDAStorage.DIDARecord rec in pair.Value)
                    {
                        Console.WriteLine("Value: " + rec.val + ", Version: (" + rec.version.versionNumber + ", " + rec.version.replicaId + ")");
                    }
                }
            }
            return new StatusReply { };
        }

        public DIDAStorage.DIDARecord Read(string id, DIDAStorage.DIDAVersion version)
        {
            List<DIDAStorage.DIDARecord> recordValues;
            DIDAStorage.DIDARecord value = new DIDAStorage.DIDARecord
            { // null value
                id = id,
                val = "",
                version = new DIDAStorage.DIDAVersion
                {
                    replicaId = -1,
                    versionNumber = -1
                }
            };

            lock (this)
            {
                Console.WriteLine("Reading... " + id);

                if (storage.TryGetValue(id, out recordValues))
                {
                    if (version.replicaId == -1 && version.versionNumber == -1)
                    { // Null version
                        int size = recordValues.Count;
                        value = recordValues[size - 1];
                        // The list is ordered
                    }
                    else
                    { // Specified version
                        foreach (DIDAStorage.DIDARecord record in recordValues)
                        {
                            // Get the most recent or the indicated version
                            if (
                                record.version.replicaId == version.replicaId &&
                                record.version.versionNumber == version.versionNumber)
                            {
                                value = record;
                                break;
                            }
                        }
                    }
                }
            }

            return value;
        }

        public async Task<DIDAVersion> UpdateIfValueIs(string id, string oldvalue, string newvalue)
        {
            lock (this)
            {
                if (!this.storage.ContainsKey(id))
                {
                    this.AddNewKey(id);
                    this.replicaManager.CreateNewEmptyTimeStamp(this.replicaId, id);
                }
                if (consensusLock[id].locked)
                {
                    return new DIDAVersion
                    {
                        ReplicaId = -1,
                        VersionNumber = -1
                    };
                }
                consensusLock[id].SetLocked(true);
            }

            // storageNodes are never removed, so we can always use a new Consistent Hashing instance
            List<string> storageIds = new List<string>();
            foreach (StorageNodeStruct sns in storageNodes.Values)
            {
                storageIds.Add(sns.serverId);
            }
            storageIds.Add(this.serverId);

            ConsistentHashing consistentHashing = new ConsistentHashing(storageIds);
            List<string> setOfReplicas = consistentHashing.ComputeSetOfReplicas(id);

            List<AsyncUnaryCall<LockAndPullReply>> lockingTasks = new List<AsyncUnaryCall<LockAndPullReply>>();
            List<LockAndPullReply> replies = new List<LockAndPullReply>();
            LockAndPullRequest lockRequest = new LockAndPullRequest { Key = id };

            foreach (string sId in setOfReplicas)
            {
                // Dont request to ourselves
                if (sId == this.serverId) continue;

                try
                {
                    lockingTasks.Add(storageNodes[sId].uiviClient.LockAndPullAsync(lockRequest));
                }
                catch (RpcException e)
                {
                    Console.WriteLine("Status Code: " + e.StatusCode);
                    // if (e.StatusCode == StatusCode.)
                }

            }

            // Await for all lock requests
            bool replicaDown = false;
            foreach (AsyncUnaryCall<LockAndPullReply> call in lockingTasks)
            {
                try
                {
                    replies.Add(await call.ResponseAsync);
                }
                catch (RpcException e)
                {
                    replicaDown = true;
                }
            }

            CommitPhaseRequest commitRequest;
            DIDAStorage.DIDAVersion nextVersion;
            DIDARecord maxRecord = new DIDARecord
            {
                Id = id,
                Val = "1",
                Version = new DIDAVersion
                {
                    VersionNumber = -1,
                    ReplicaId = -1
                }
            };

            lock (this)
            {
                bool abort = false;

                if (!replicaDown)
                {
                    // This replica starts being the max version
                    if (this.storage[id].Count > 0)
                    {
                        DIDAStorage.DIDARecord record = this.storage[id][this.storage[id].Count - 1];
                        maxRecord = new DIDARecord
                        {
                            Id = record.id,
                            Val = record.val,
                            Version = new DIDAVersion
                            {
                                VersionNumber = record.version.versionNumber,
                                ReplicaId = record.version.replicaId
                            }
                        };
                    }

                    foreach (LockAndPullReply reply in replies)
                    {
                        if (reply.AlreadyLocked)
                        {
                            abort = true;
                            break;
                        }
                        if (this.replicaManager.IsVersionBigger(reply.Record.Version, maxRecord.Version))
                        {
                            maxRecord = reply.Record;
                        }
                    }
                }

                if (!abort)
                {
                    nextVersion = new DIDAStorage.DIDAVersion
                    {
                        versionNumber = maxRecord.Version.VersionNumber + 1,
                        replicaId = this.replicaId
                    };

                    DIDAStorage.DIDARecord nextRecord = new DIDAStorage.DIDARecord
                    {
                        id = maxRecord.Id,
                        val = newvalue,
                        version = nextVersion
                    };

                    if (maxRecord.Val == oldvalue) // If VALUE IS
                    {
                        List<DIDAStorage.DIDARecord> items = storage[id];
                        // If Queue is full
                        if (items.Count == MAX_VERSIONS_STORED)
                        {
                            items.RemoveAt(0);
                        }
                        items.Add(nextRecord);
                        this.replicaManager.AddTimeStamp(this.replicaId, id, nextVersion);

                        commitRequest = new CommitPhaseRequest
                        {
                            CanCommit = true,
                            Record = new DIDARecord
                            {
                                Id = nextRecord.id,
                                Val = nextRecord.val,
                                Version = new DIDAVersion
                                {
                                    VersionNumber = nextVersion.versionNumber,
                                    ReplicaId = nextVersion.replicaId
                                }
                            }
                        };
                    }
                    else
                    {
                        commitRequest = new CommitPhaseRequest
                        {
                            CanCommit = false,
                            Record = maxRecord
                        };
                        nextVersion = new DIDAStorage.DIDAVersion
                        {
                            replicaId = -1,
                            versionNumber = -1
                        };
                    }
                }
                else
                {
                    commitRequest = new CommitPhaseRequest
                    {
                        CanCommit = false,
                        Record = maxRecord
                    };
                    nextVersion = new DIDAStorage.DIDAVersion
                    {
                        replicaId = -1,
                        versionNumber = -1
                    };
                }
                consensusLock[id].SetLocked(false);
            }

            foreach (string sId in setOfReplicas)
            {
                // Dont request to ourselves
                if (sId == this.serverId) continue;

                try
                {
                    storageNodes[sId].uiviClient.CommitPhaseAsync(commitRequest);
                }
                catch (RpcException e)
                {
                    Console.WriteLine("Status Code: " + e.StatusCode);
                }
            }

            return new DIDAVersion
            {
                VersionNumber = nextVersion.versionNumber,
                ReplicaId = nextVersion.replicaId
            };
        }

        public LockAndPullReply LockAndPull(LockAndPullRequest request)
        {
            DIDARecord maxRecord;

            lock (this)
            {
                if (!this.storage.ContainsKey(request.Key))
                {
                    this.AddNewKey(request.Key);
                    this.replicaManager.CreateNewEmptyTimeStamp(this.replicaId, request.Key);
                }

                if (consensusLock[request.Key].locked)
                {
                    return new LockAndPullReply { AlreadyLocked = true };
                }
                consensusLock[request.Key].SetLocked(true);

                if (this.storage[request.Key].Count > 0)
                {
                    DIDAStorage.DIDARecord record = this.storage[request.Key][this.storage[request.Key].Count - 1];
                    maxRecord = new DIDARecord
                    {
                        Id = record.id,
                        Val = record.val,
                        Version = new DIDAVersion
                        {
                            VersionNumber = record.version.versionNumber,
                            ReplicaId = record.version.replicaId
                        }
                    };
                }
                else
                {
                    maxRecord = new DIDARecord
                    {
                        Id = request.Key,
                        Val = "1",
                        Version = new DIDAVersion
                        {
                            VersionNumber = -1,
                            ReplicaId = -1
                        }
                    };
                }
            }

            this.consensusLock[request.Key].timer.Start();
            return new LockAndPullReply { AlreadyLocked = false, Record = maxRecord };
        }

        public CommitPhaseReply CommitPhase(CommitPhaseRequest request)
        {
            lock (this)
            {

                if (!this.storage.ContainsKey(request.Record.Id))
                {
                    this.AddNewKey(request.Record.Id);
                    this.replicaManager.CreateNewEmptyTimeStamp(this.replicaId, request.Record.Id);
                }

                this.consensusLock[request.Record.Id].timer.Stop();

                if (!request.CanCommit)
                {
                    consensusLock[request.Record.Id].SetLocked(false);
                    return new CommitPhaseReply { Okay = true };
                }

                DIDAStorage.DIDAVersion newVersion = new DIDAStorage.DIDAVersion
                {
                    replicaId = request.Record.Version.ReplicaId,
                    versionNumber = request.Record.Version.VersionNumber
                };

                bool inserted = false;

                DIDAStorage.DIDARecord newRecord = new DIDAStorage.DIDARecord
                {
                    id = request.Record.Id,
                    version = newVersion,
                    val = request.Record.Val
                };

                for (int i = 0; i < this.storage[request.Record.Id].Count; i++)
                {
                    if (this.replicaManager.IsVersionEqual(this.storage[request.Record.Id][i].version, newVersion))
                    {
                        inserted = true;
                        break;
                    }

                    if (this.replicaManager.IsVersionBigger(this.storage[request.Record.Id][i].version, newVersion))
                    {
                        this.storage[request.Record.Id].Insert(i, newRecord);
                        this.replicaManager.AddTimeStamp(replicaId, request.Record.Id, newVersion);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    this.storage[request.Record.Id].Add(newRecord);
                    this.replicaManager.AddTimeStamp(replicaId, request.Record.Id, newVersion);
                }

                if (this.storage[request.Record.Id].Count > MAX_VERSIONS_STORED)
                {
                    this.storage[request.Record.Id].RemoveAt(0);
                }

                consensusLock[request.Record.Id].SetLocked(false);
            }

            return new CommitPhaseReply { Okay = true };
        }

        public DIDAStorage.DIDAVersion Write(string id, string val)
        {
            DIDAStorage.DIDAVersion didaVersion;
            List<DIDAStorage.DIDARecord> recordValues;

            lock (this)
            {

                if (consensusLock[id].locked)
                {
                    return new DIDAStorage.DIDAVersion
                    {
                        replicaId = -1,
                        versionNumber = -1
                    };
                }

                if (!this.startedGossip)
                {
                    this.startedGossip = true;
                    timer.Start();
                }
                Console.WriteLine("Writing... " + id + " - " + val);

                // Get the greater version
                int greaterVersionNumber = 0;
                if (storage.TryGetValue(id, out recordValues))
                {
                    int size = recordValues.Count;
                    greaterVersionNumber = recordValues[size - 1].version.versionNumber;
                }

                didaVersion = new DIDAStorage.DIDAVersion
                {
                    replicaId = this.replicaId,
                    versionNumber = ++greaterVersionNumber
                };

                DIDAStorage.DIDARecord didaRecord = new DIDAStorage.DIDARecord
                {
                    id = id,
                    version = didaVersion,
                    val = val
                };

                if (storage.ContainsKey(id))
                {
                    List<DIDAStorage.DIDARecord> items = storage[id];

                    // If Queue is full
                    if (items.Count == MAX_VERSIONS_STORED)
                    {
                        items.RemoveAt(0);
                    }
                    items.Add(didaRecord);
                    this.replicaManager.AddTimeStamp(this.replicaId, id, didaVersion);
                }
                else
                {
                    //storage.Add(id, new List<DIDAStorage.DIDARecord>());
                    this.AddNewKey(id);
                    storage[id].Add(didaRecord);
                    this.replicaManager.CreateNewTimeStamp(
                        this.replicaId,
                        id,
                        didaVersion
                    );
                }
            }

            return didaVersion;
        }

        public PopulateReply Populate(RepeatedField<KeyValuePair> keyValuePairs)
        {
            foreach (KeyValuePair pair in keyValuePairs)
            {
                DIDAStorage.DIDAVersion version;
                version.replicaId = pair.ReplicaId;
                version.versionNumber = 1;

                DIDAStorage.DIDARecord record;
                record.id = pair.Key;
                record.val = pair.Value;
                record.version = version;

                this.AddNewKey(record.id);
                this.storage[record.id].Add(record);

                this.replicaManager.CreateNewTimeStamp(
                    this.replicaId,
                    pair.Key,
                    new DIDAStorage.DIDAVersion
                    {
                        versionNumber = version.versionNumber,
                        replicaId = version.replicaId
                    }
                );

                Console.WriteLine("Stored!! Key: " + pair.Key + " Value: " + storage[pair.Key][0].val + " Id: " + storage[pair.Key][0].id + " Version Number: " + storage[pair.Key][0].version.versionNumber + " Replica Id: " + storage[pair.Key][0].version.replicaId);
            }

            return new PopulateReply { Okay = true };
        }

        public List<DIDAStorage.DIDARecord> Dump()
        {
            List<DIDAStorage.DIDARecord> data = new List<DIDAStorage.DIDARecord>();

            lock (storage)
            {
                foreach (string key in storage.Keys)
                {
                    var records = storage[key];
                    foreach (DIDAStorage.DIDARecord record in records)
                    {
                        data.Add(record);
                    }
                }
            }

            return data;
        }

        public AddStorageReply AddStorage(RepeatedField<StorageInfo> storageInfos)
        {
            foreach (StorageInfo si in storageInfos)
            {
                StorageNodeStruct node;
                node.serverId = si.ServerId;
                node.replicaId = si.ReplicaId;
                node.url = si.Url;
                node.channel = GrpcChannel.ForAddress(node.url);
                node.gossipClient = new GossipService.GossipServiceClient(node.channel);
                node.uiviClient = new UpdateIfValueIsService.UpdateIfValueIsServiceClient(node.channel);

                Console.WriteLine("Added Storage server Id: " + node.serverId);
                storageNodes.Add(node.serverId, node);

                this.replicaManager.AddStorageReplica(node.replicaId);
            }

            return new AddStorageReply { Okay = true };
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Calling Gossip...");
            this.Gossip();
        }

        public void Gossip()
        {
            lock (this)
            {
                List<string> keys = this.replicaManager.GetMyKeys();
                Dictionary<string, List<DIDAStorage.DIDAVersion>> myReplicaTimestamp = this.replicaManager.GetReplicaTimeStamp(this.replicaId);
                Dictionary<string, GossipRequest> gossipRequests = new Dictionary<string, GossipRequest>();

                List<string> storageIds = new List<string>();
                foreach (StorageNodeStruct sns in storageNodes.Values)
                {
                    storageIds.Add(sns.serverId);
                }
                storageIds.Add(this.serverId);

                foreach (string key in keys)
                {
                    ConsistentHashing consistentHashing = new ConsistentHashing(storageIds);
                    List<string> setOfReplicas = consistentHashing.ComputeSetOfReplicas(key);

                    foreach (string sId in setOfReplicas)
                    {
                        // Dont gossip to ourselves
                        if (sId == this.serverId) continue;

                        if (!gossipRequests.ContainsKey(sId))
                        {
                            gossipRequests.Add(sId, new GossipRequest { ReplicaId = this.replicaId });
                        }
                        StorageNodeStruct sns = this.storageNodes[sId];
                        Dictionary<string, List<DIDAStorage.DIDAVersion>> snsReplicaTimestamp = this.replicaManager.GetReplicaTimeStamp(sns.replicaId);
                        if (!snsReplicaTimestamp.ContainsKey(key))
                        {
                            snsReplicaTimestamp.Add(key, new List<DIDAStorage.DIDAVersion>());
                        }

                        List<DIDAStorage.DIDAVersion> timestampsToSend = this.replicaManager.ComputeGossipTimestampsDifferences(myReplicaTimestamp[key], snsReplicaTimestamp[key]);

                        foreach (DIDAStorage.DIDAVersion snsVersion in timestampsToSend)
                        {
                            foreach (DIDAStorage.DIDARecord record in this.storage[key])
                            {
                                if (record.version.versionNumber == snsVersion.versionNumber && record.version.replicaId == snsVersion.replicaId)
                                {
                                    gossipRequests[sId].UpdateLogs.Add(new DIDARecord
                                    {
                                        Id = key,
                                        Val = record.val,
                                        Version = new DIDAVersion
                                        {
                                            VersionNumber = record.version.versionNumber,
                                            ReplicaId = record.version.replicaId
                                        }
                                    });
                                    break;
                                }
                            }
                        }

                        var grpcTimestamp = new TimeStamp { Key = key };
                        foreach (DIDAStorage.DIDAVersion version in myReplicaTimestamp[key])
                        {
                            grpcTimestamp.Timestamp.Add(new DIDAVersion
                            {
                                VersionNumber = version.versionNumber,
                                ReplicaId = version.replicaId
                            });
                        }
                        gossipRequests[sId].ReplicaTimestamp.Add(grpcTimestamp);
                    }
                }

                foreach (string requestServerId in gossipRequests.Keys)
                {
                    try
                    {
                        GossipRequest request = gossipRequests[requestServerId];
                        if (request.UpdateLogs.Count > 0)
                        {
                            Console.WriteLine("============>>> REQUESTT <<<<========================= ServerId: " + requestServerId);
                            GossipReply reply = this.storageNodes[requestServerId].gossipClient.Gossip(request);
                            this.replicaManager.ReplaceTimeStamp(this.storageNodes[requestServerId].replicaId, reply.ReplicaTimestamp);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Gossip Requests exception: " + e.Message);
                    }
                }
            }
        }

        public GossipReply ReceiveGossip(GossipRequest request)
        {
            Console.WriteLine("Receive Gossip from: " + request.ReplicaId);
            Dictionary<string, List<DIDAStorage.DIDAVersion>> myReplicaTimestamp;
            lock (this)
            {
                int otherReplicaId = request.ReplicaId;
                myReplicaTimestamp = this.replicaManager.GetReplicaTimeStamp(this.replicaId);

                foreach (DIDARecord update in request.UpdateLogs)
                {
                    DIDAStorage.DIDAVersion updateVersion = new DIDAStorage.DIDAVersion
                    {
                        versionNumber = update.Version.VersionNumber,
                        replicaId = update.Version.ReplicaId
                    };

                    if (!this.storage.ContainsKey(update.Id))
                    {
                        this.AddNewKey(update.Id);
                        this.replicaManager.CreateNewEmptyTimeStamp(otherReplicaId, update.Id);
                    }

                    bool inserted = false;

                    DIDAStorage.DIDARecord newRecord = new DIDAStorage.DIDARecord
                    {
                        id = update.Id,
                        version = updateVersion,
                        val = update.Val
                    };

                    for (int i = 0; i < this.storage[update.Id].Count; i++)
                    {
                        if (this.replicaManager.IsVersionEqual(this.storage[update.Id][i].version, updateVersion))
                        {
                            inserted = true;
                            break;
                        }

                        if (this.replicaManager.IsVersionBigger(this.storage[update.Id][i].version, updateVersion))
                        {
                            this.storage[update.Id].Insert(i, newRecord);
                            this.replicaManager.AddTimeStamp(replicaId, update.Id, updateVersion);
                            inserted = true;
                            break;
                        }
                    }

                    if (!inserted)
                    {
                        this.storage[update.Id].Add(newRecord);
                        this.replicaManager.AddTimeStamp(replicaId, update.Id, updateVersion);
                    }

                    if (this.storage[update.Id].Count > MAX_VERSIONS_STORED)
                    {
                        this.storage[update.Id].RemoveAt(0);
                    }
                }

                // Update other Replica Timestamp
                this.replicaManager.ReplaceTimeStamp(otherReplicaId, request.ReplicaTimestamp);

                myReplicaTimestamp = this.replicaManager.GetReplicaTimeStamp(this.replicaId);
            }

            var reply = new GossipReply { };
            foreach (string key in myReplicaTimestamp.Keys)
            {
                TimeStamp grpcTimestamp = new TimeStamp { Key = key };
                foreach (DIDAStorage.DIDAVersion version in myReplicaTimestamp[key])
                {
                    grpcTimestamp.Timestamp.Add(new DIDAVersion
                    {
                        VersionNumber = version.versionNumber,
                        ReplicaId = version.replicaId
                    });
                }
                reply.ReplicaTimestamp.Add(grpcTimestamp);
            }

            return reply;
        }

        public void FreeConsensusKey(string key)
        {
            Console.WriteLine("[ TIMEOUT ] - Free lock on key: " + key);
            this.consensusLock[key].SetLocked(false);
        }

        private void AddNewKey(string key)
        {
            this.storage.Add(key, new List<DIDAStorage.DIDARecord>());
            this.consensusLock.Add(key, new ConsensusKeyLock
            { 
                locked = false,
                timer = new Timer
                {
                    Interval = CONSENSUS_TIMEOUT,
                    AutoReset = false,
                    Enabled = false
                }
            });

            ElapsedEventHandler handler = ((sender, args) => {
                this.FreeConsensusKey(key);
            });
            this.consensusLock[key].timer.Elapsed += handler;
        }
    }
    public struct StorageNodeStruct
    {
        public string serverId;
        public string url;
        public int replicaId;
        public GrpcChannel channel;
        public GossipService.GossipServiceClient gossipClient;
        public UpdateIfValueIsService.UpdateIfValueIsServiceClient uiviClient;
    }

    public struct ConsensusKeyLock
    {
        public bool locked;
        public Timer timer;

        public void SetLocked(bool value)
        {
            this.locked = value;
        }
    }
}
