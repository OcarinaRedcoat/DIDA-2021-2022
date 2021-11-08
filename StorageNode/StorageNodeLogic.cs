using DIDAStorage;
using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class StorageNodeLogic : IDIDAStorage
    {
        public static int MAX_VERSIONS_STORED = 5;

        private Dictionary<string, StorageNodeStruct> storageNodes = new Dictionary<string, StorageNodeStruct>();

        // Must be a queue instead of a list, in order to pop old values
        private Dictionary<string, List<DIDAStorage.DIDARecord>> storage = new Dictionary<string, List<DIDAStorage.DIDARecord>>();
        private ReplicaManager replicaManager;
        private int replicaId;
        private int gossipDelay;
        public StorageNodeLogic(int replicaId, int gossipDelay)
        {
            this.replicaId = replicaId;
            this.gossipDelay = gossipDelay;
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

            lock (storage) 
            {
                Console.WriteLine("Reading... " + id);

                if (storage.TryGetValue(id, out recordValues))
                {
                    if (version.replicaId == -1 && version.versionNumber == -1)
                    { // Null version
                        int size = recordValues.Count;
                        value = recordValues[size - 1];
                        // We suppose the list is ordered
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

        public DIDAStorage.DIDAVersion UpdateIfValueIs(string id, string oldvalue, string newvalue)
        {
            throw new NotImplementedException();
        }

        public DIDAStorage.DIDAVersion Write(string id, string val)
        {
            DIDAStorage.DIDAVersion didaVersion;
            List<DIDAStorage.DIDARecord> recordValues;

            lock (storage)
            {
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
                }
                else
                {
                    storage.Add(id, new List<DIDAStorage.DIDARecord>());
                    storage[id].Add(didaRecord);
                }
            }

            return didaVersion;
        }

        public PopulateReply PopulateSerialize(RepeatedField<KeyValuePair> keyValuePairs)
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

                List<DIDAStorage.DIDARecord> listRecords = new List<DIDAStorage.DIDARecord>();
                listRecords.Add(record);
                storage.Add(pair.Key, listRecords);

                Console.WriteLine("Stored!! Key: " + pair.Key + " Value: " + storage[pair.Key][0].val + " Id: " + storage[pair.Key][0].id + " Version Number: " + storage[pair.Key][0].version.versionNumber + " Replica Id: " + storage[pair.Key][0].version.replicaId);
            }

            return new PopulateReply { Okay = true };
        }

        public PopulateReply Populate(RepeatedField<KeyValuePair> keyValuePairs)
        {
            return PopulateSerialize(keyValuePairs);
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

                Console.WriteLine("Added Storage server Id: " + node.serverId);
            }

            SetupReplicaManager();

            return new AddStorageReply { Okay = true };
        }

        public void SetupReplicaManager()
        {
            List<StorageNodeStruct> storageNodesList = new List<StorageNodeStruct>();
            foreach (StorageNodeStruct sns in this.storageNodes.Values)
            {
                storageNodesList.Add(sns);
            } 
            this.replicaManager = new ReplicaManager(this.replicaId, this.gossipDelay, storageNodesList);
        }

        public void Gossip()
        {
            List<string> keys = this.replicaManager.GetMyKeys();
            Dictionary<string, DIDAStorage.DIDAVersion> myTimestamp = this.replicaManager.GetMyReplicaTimeStamp();
            Dictionary<string, GossipRequest> gossipRequests = new Dictionary<string, GossipRequest>();
            Dictionary<string, DIDAStorage.DIDAVersion> myReplicaTS = this.replicaManager.GetMyReplicaTimeStamp();

            foreach (string key in keys)
            {
                List<string> setOfReplicas = this.replicaManager.ComputeSetOfReplicas(key);
                foreach (string serverId in setOfReplicas)
                {
                    if (!gossipRequests.ContainsKey(serverId))
                    {
                        gossipRequests.Add(serverId, new GossipRequest { ReplicaId = this.replicaId });
                    }
                    StorageNodeStruct sns = this.storageNodes[serverId];
                    Dictionary<string, DIDAStorage.DIDAVersion> snsTimestamp = this.replicaManager.GetReplicaTimeStamp(sns.replicaId);
                    if (this.replicaManager.VersionIsBiggerComparator(myTimestamp[key], snsTimestamp[key]))
                    {
                        // TODO: Dont send everything
                        foreach (DIDAStorage.DIDARecord record in this.storage[serverId])
                        {
                            gossipRequests[serverId].UpdateLogs.Add(new DIDARecord
                            {
                                Id = key,
                                Val = record.val,
                                Version = new DIDAVersion
                                {
                                    VersionNumber = record.version.versionNumber,
                                    ReplicaId = record.version.replicaId
                                }
                            });
                        }
                    }

                    gossipRequests[serverId].ReplicaTimestamp.Add(new TimeStamp
                        {
                            Key = key,
                            ReplicaTimestamp = new DIDAVersion
                            {
                                VersionNumber = myReplicaTS[key].versionNumber,
                                ReplicaId = myReplicaTS[key].replicaId
                            }
                        }
                    );
                }
            }

            foreach (string requestServerId in gossipRequests.Keys)
            {
                GossipRequest request = gossipRequests[requestServerId];
                GossipReply reply = this.storageNodes[requestServerId].gossipClient.Gossip(request);
                this.replicaManager.UpdateReplicaTimeStamp(this.storageNodes[requestServerId].replicaId, reply.ReplicaTimestamp);
            }
        }

        public GossipReply ReceiveGossip(GossipRequest request)
        {
            Dictionary<string, DIDAStorage.DIDAVersion> myReplicaTS;
            lock (this)
            {
                int otherReplicaId = request.ReplicaId;
                myReplicaTS = this.replicaManager.GetMyReplicaTimeStamp();

                List<DIDAStorage.DIDARecord> updateLogs = new List<DIDAStorage.DIDARecord>();

                foreach (DIDARecord record in request.UpdateLogs)
                {
                    updateLogs.Add(new DIDAStorage.DIDARecord
                    {
                        id = record.Id,
                        val = record.Val,
                        version = new DIDAStorage.DIDAVersion
                        {
                            versionNumber = record.Version.VersionNumber,
                            replicaId = record.Version.ReplicaId
                        }
                    });
                }

                foreach (DIDAStorage.DIDARecord update in updateLogs)
                {
                    if (!this.storage.ContainsKey(update.id))
                    {
                        this.storage.Add(update.id, new List<DIDAStorage.DIDARecord>());
                        this.replicaManager.AddNewKeyTimeStamp(update.id, new DIDAStorage.DIDAVersion { versionNumber = -1, replicaId = -1 });
                    }

                    if (this.replicaManager.VersionIsBiggerComparator(update.version, myReplicaTS[update.id]))
                    {
                        this.replicaManager.AddNewLog(update.id, update);
                        if (this.storage[update.id].Count > MAX_VERSIONS_STORED)
                        {
                            this.storage[update.id].RemoveAt(0);
                        }
                        this.storage[update.id].Add(update);
                        this.replicaManager.UpdateTimeStamp(replicaId, update.id, this.storage[update.id][0].version);
                    }
                }

                // Update other Replica Timestamp
                this.replicaManager.UpdateReplicaTimeStamp(otherReplicaId, request.ReplicaTimestamp);

                myReplicaTS = this.replicaManager.GetMyReplicaTimeStamp();
            }

            // TODO: Send only the needed keys
            var reply =  new GossipReply {};
            foreach (string key in myReplicaTS.Keys)
            {
                reply.ReplicaTimestamp.Add(new TimeStamp
                {
                    Key = key,
                    ReplicaTimestamp = new DIDAVersion
                    {
                        VersionNumber = myReplicaTS[key].versionNumber,
                        ReplicaId = myReplicaTS[key].replicaId
                    }
                });
            }
            
            return reply;
        }
    }
    public struct StorageNodeStruct
    {
        public string serverId;
        public string url;
        public int replicaId;
        public GrpcChannel channel;
        public GossipService.GossipServiceClient gossipClient;
    }
}
