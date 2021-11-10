using System;
using System.Collections.Generic;
using System.Text;
using DIDAStorageClient;
using DIDAWorker;
using CHashing;
using System.Threading;

namespace WorkerNode
{
    public class StorageProxy : IDIDAStorage
    {
        // dictionary with storage gRPC client objects for all storage nodes
        Dictionary<string, DIDAStorageService.DIDAStorageServiceClient> _clients = new Dictionary<string, DIDAStorageService.DIDAStorageServiceClient>();

        // dictionary with storage gRPC channel objects for all storage nodes
        Dictionary<string, Grpc.Net.Client.GrpcChannel> _channels = new Dictionary<string, Grpc.Net.Client.GrpcChannel>();

        // metarecord for the request that this storage proxy is handling
        DIDAMetaRecord _meta;

        ConsistentHashing _consistentHashing;

        // The constructor of a storage proxy.
        // The storageNodes parameter lists the nodes that this storage proxy needs to be aware of to perform
        // read, write and updateIfValueIs operations.
        // The metaRecord identifies the request being processed by this storage proxy object
        // and allows the storage proxy to request data versions previously accessed by the request
        // and to inform operators running on the following (downstream) workers of the versions it accessed.
        public StorageProxy(DIDAStorageNode[] storageNodes, DIDAMetaRecord metaRecord)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            List<string> storageIds = new List<string>();
            foreach (DIDAStorageNode n in storageNodes)
            {
                _channels[n.serverId] = Grpc.Net.Client.GrpcChannel.ForAddress("http://" + n.host + ":" + n.port);
                _clients[n.serverId] = new DIDAStorageService.DIDAStorageServiceClient(_channels[n.serverId]);
                storageIds.Add(n.serverId);
            }
            _meta = metaRecord;
            _consistentHashing = new ConsistentHashing(storageIds);
        }

        // THE FOLLOWING 3 METHODS ARE THE ESSENCE OF A STORAGE PROXY
        // IN THIS EXAMPLE THEY ARE JUST CALLING THE STORAGE 
        // IN THE COMLPETE IMPLEMENTATION THEY NEED TO:
        // 1) LOCATE THE RIGHT STORAGE SERVER
        // 2) DEAL WITH FAILED STORAGE SERVERS
        // 3) CHECK IN THE METARECORD WHICH ARE THE PREVIOUSLY READ VERSIONS OF DATA 
        // 4) RECORD ACCESSED DATA INTO THE METARECORD

        // this dummy solution assumes there is a single storage server called "s1"
        public virtual DIDAWorker.DIDARecordReply read(DIDAWorker.DIDAReadRequest r)
        {
            // TODO: META INFORMATION ABOUT VERSIONS READ AND WRITE
            List<string> storagesIds = _consistentHashing.ComputeShuffledSetOfReplicas(r.Id);
            DIDAStorageClient.DIDARecordReply res;
            DIDAVersion prevMetaVersion = null;

            
            foreach (KeyVersionAccess access in _meta.PreviousAccessed)
            {
                if (r.Id == access.Key)
                {
                    prevMetaVersion = access.Version;
                }
            }

            foreach (string sId in storagesIds)
            {
                try
                {
                    Console.WriteLine("Calling Read on... " + sId);
                    res = _clients[sId].read(
                        new DIDAStorageClient.DIDAReadRequest {
                            Id = r.Id,
                            Version = new DIDAStorageClient.DIDAVersion
                            {
                                VersionNumber = prevMetaVersion != null ? prevMetaVersion.VersionNumber : r.Version.VersionNumber,
                                ReplicaId = prevMetaVersion != null ? prevMetaVersion.ReplicaId : r.Version.ReplicaId
                            }
                        }
                    );
                    // If the replica does not have the version required, try another one!
                    if (res.Version.ReplicaId == -1 && res.Version.VersionNumber == -1)
                    {
                        Console.WriteLine("Replica does not have the version required...");
                        continue;
                    }
                    // ADD ACCESS TO METARECORD
                    bool existsInMeta = false;
                    foreach (KeyVersionAccess access in _meta.PreviousAccessed)
                    {
                        if (access.Key == r.Id)
                        {
                            existsInMeta = true;
                            access.Version = new DIDAVersion { VersionNumber = res.Version.VersionNumber, ReplicaId = res.Version.ReplicaId };
                        }
                    }
                    if (!existsInMeta)
                    {
                        _meta.PreviousAccessed.Add(new KeyVersionAccess
                        {
                            Key = r.Id,
                            Version = new DIDAVersion
                            {
                                VersionNumber = res.Version.VersionNumber,
                                ReplicaId = res.Version.ReplicaId
                            }
                        });
                    }
                    Console.WriteLine("Read Succeed: " + sId);
                    return new DIDAWorker.DIDARecordReply
                    {
                        Id = res.Id,
                        Val = res.Val,
                        Version = new DIDAWorker.DIDAVersion
                        {
                            VersionNumber = res.Version.VersionNumber,
                            ReplicaId = res.Version.ReplicaId
                        }
                    };
                }
                catch (Exception e)
                {
                    // Replica down
                    // TODO: Remove the replica!!!
                    Console.WriteLine("Read Failed to Replica: " + sId);
                    Console.WriteLine(e.Message);
                }
            }
            // res = _clients["s1"].read(new DIDAStorageClient.DIDAReadRequest { Id = r.Id, Version = new DIDAStorageClient.DIDAVersion { VersionNumber = r.Version.VersionNumber, ReplicaId = r.Version.ReplicaId } });
            Console.WriteLine("READ NOT FOUND ABORTING");
            Thread.CurrentThread.Abort();
            return new DIDAWorker.DIDARecordReply { Id = "1", Val = "1", Version = { VersionNumber = -1, ReplicaId = -1 } };
        }

        // this dummy solution assumes there is a single storage server called "s1"
        public virtual DIDAWorker.DIDAVersion write(DIDAWorker.DIDAWriteRequest r)
        {
            List<string> storagesIds = _consistentHashing.ComputeShuffledSetOfReplicas(r.Id);
            DIDAStorageClient.DIDAVersion res;
            foreach (string sId in storagesIds)
            {
                try
                {
                    Console.WriteLine("Calling Write on... " + sId);
                    res = _clients[sId].write(
                        new DIDAStorageClient.DIDAWriteRequest
                        {
                            Id = r.Id,
                            Val = r.Val
                        }
                    );
                    // ADD ACCESS TO METARECORD
                    bool existsInMeta = false;
                    foreach (KeyVersionAccess access in _meta.PreviousAccessed)
                    {
                        if (access.Key == r.Id)
                        {
                            existsInMeta = true;
                            access.Version = new DIDAVersion { VersionNumber = res.VersionNumber, ReplicaId = res.ReplicaId };
                        }
                    }
                    if (!existsInMeta)
                    {
                        _meta.PreviousAccessed.Add(new KeyVersionAccess {
                            Key = r.Id,
                            Version = new DIDAVersion
                            {
                                VersionNumber = res.VersionNumber,
                                ReplicaId = res.ReplicaId
                            }
                        });
                    }
                    Console.WriteLine("Write Succeed: " + sId);
                    return new DIDAWorker.DIDAVersion
                    {
                        VersionNumber = res.VersionNumber,
                        ReplicaId = res.ReplicaId
                    };
                }
                catch (Exception e)
                {
                    // Replica down
                    // TODO: Remove the replica!!!
                    Console.WriteLine("Write Failed to Replica: " + sId);
                    Console.WriteLine(e.Message);
                }
            }
            // var res = _clients["s1"].write(new DIDAStorageClient.DIDAWriteRequest { Id = r.Id, Val = r.Val });
            // return new DIDAWorker.DIDAVersion { VersionNumber = res.VersionNumber, ReplicaId = res.ReplicaId };

            Console.WriteLine("WRITE NOT POSSIBLE ABORTING");
            Thread.CurrentThread.Abort();
            return new DIDAWorker.DIDAVersion { VersionNumber = -1, ReplicaId = -1 };
        }

        // this dummy solution assumes there is a single storage server called "s1"
        public virtual DIDAWorker.DIDAVersion updateIfValueIs(DIDAWorker.DIDAUpdateIfRequest r)
        {
            List<string> storagesIds = _consistentHashing.ComputeShuffledSetOfReplicas(r.Id);

            foreach (string sId in storagesIds)
            {
                try
                {
                    Console.WriteLine("Calling UpdateIfValueIs on... " + sId);
                    Console.WriteLine("Id: :" + r.Id);
                    Console.WriteLine("newvalue: " + r.Newvalue);
                    Console.WriteLine("oldvalue: " + r.Oldvalue);
                    DIDAStorageClient.DIDAVersion res = _clients[sId].updateIfValueIs(
                        new DIDAStorageClient.DIDAUpdateIfRequest
                        {
                            Id = r.Id,
                            Newvalue = r.Newvalue,
                            Oldvalue = r.Oldvalue
                        }
                    );

                    // TODO: TIMEOUT in GRPC setting????
                    // if null object retry next replica
                    if (res.ReplicaId == -1 && res.VersionNumber == -1)
                    {
                        Console.WriteLine("Replica " + sId +  " aborted");
                        continue;
                    }

                    Console.WriteLine("UpdateIfValueIs Succeed: " + sId);
                    return new DIDAWorker.DIDAVersion
                    {
                        VersionNumber = res.VersionNumber,
                        ReplicaId = res.ReplicaId
                    };
                }
                catch (Exception e)
                {
                    // TIMEOUT EXCEPTION VS REPLICA DOWN

                    // Replica down
                    // TODO: Remove the replica!!!
                    Console.WriteLine("UpdateIfValueIs Failed to Replica: " + sId);
                    Console.WriteLine(e.Message);
                }
            }
            Console.WriteLine("UPDATE IF VALUE IS NOT POSSIBLE ABORTING");
            Thread.CurrentThread.Abort();
            return new DIDAWorker.DIDAVersion { VersionNumber = -1, ReplicaId = -1 };
        }

        public DIDAMetaRecord GetMetaRecord()
        {
            return _meta;
        }

    }
}
