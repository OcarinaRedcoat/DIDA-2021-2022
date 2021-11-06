using System;
using System.Collections.Generic;
using System.Text;
using DIDAStorageClient;
using DIDAWorker;
using System.Security.Cryptography;

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
            _consistentHashing = new StorageProxy.ConsistentHashing(storageIds);
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
            // TODO: How to choose the storage function?
            List<string> storagesIds = _consistentHashing.ComputeSetOfReplicas(r.Id);
            Console.WriteLine("Calling Read...");
            foreach(string sId in storagesIds)
            {
                Console.WriteLine("StorageId: " + sId);
            }
            var res = _clients["s1"].read(new DIDAStorageClient.DIDAReadRequest { Id = r.Id, Version = new DIDAStorageClient.DIDAVersion { VersionNumber = r.Version.VersionNumber, ReplicaId = r.Version.ReplicaId } });
            return new DIDAWorker.DIDARecordReply { Id = "1", Val = "1", Version = { VersionNumber = 1, ReplicaId = 1 } };
        }

        // this dummy solution assumes there is a single storage server called "s1"
        public virtual DIDAWorker.DIDAVersion write(DIDAWorker.DIDAWriteRequest r)
        {
            // TODO: How to choose the storage function?
            var res = _clients["s1"].write(new DIDAStorageClient.DIDAWriteRequest { Id = r.Id, Val = r.Val });
            return new DIDAWorker.DIDAVersion { VersionNumber = res.VersionNumber, ReplicaId = res.ReplicaId };
        }

        // this dummy solution assumes there is a single storage server called "s1"
        public virtual DIDAWorker.DIDAVersion updateIfValueIs(DIDAWorker.DIDAUpdateIfRequest r)
        {
            // TODO: How to choose the storage function?
            var res = _clients["s1"].updateIfValueIs(new DIDAStorageClient.DIDAUpdateIfRequest { Id = r.Id, Newvalue = r.Newvalue, Oldvalue = r.Oldvalue });
            return new DIDAWorker.DIDAVersion { VersionNumber = res.VersionNumber, ReplicaId = res.ReplicaId };
        }

        class ConsistentHashing {

            private int RING_SIZE = 256;
            private List<HashStorageNode> storageHashIds = new List<HashStorageNode>();
            private int numOfServers;
            private int replicationFactor = 3;
            private Random rng = new Random();

            public ConsistentHashing(List<string> storageKeys)
            {
                numOfServers = storageKeys.Count;
                int storageIdx = 1;
                foreach (string key in storageKeys)
                {
                    this.storageHashIds.Add(new HashStorageNode
                    {
                        serverId = key,
                        hashUpperBound = storageIdx == numOfServers ? RING_SIZE : NodeHash(storageIdx)
                    });
                    storageIdx++;
                }
                this.display();
            }

            public void display()
            {
                foreach (HashStorageNode node in storageHashIds)
                {
                    Console.WriteLine("StorageId: " + node.serverId);
                    Console.WriteLine("StorageHash: " + node.hashUpperBound);
                }
            }

            public int NodeHash(int idx)
            {
                return (int) (idx * Math.Floor((double) (RING_SIZE / numOfServers)));
            }

            public int KeyHash(string key)
            {
                // Create a SHA256   
                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(key));
                    // We only need the last byte (bytes mod 256) == byte[-1]
                    return bytes[bytes.Length - 1];
                }
            }

            public List<string> ComputeSetOfReplicas(string key)
            {
                int keyHash = KeyHash(key);
                List<string> setOfReplicas = new List<string>();
                bool found = false;

                for (int i = 0; setOfReplicas.Count < replicationFactor; i = (i+1) % storageHashIds.Count)
                {
                    if (keyHash < storageHashIds[i].hashUpperBound && !found)
                    {
                        setOfReplicas.Add(storageHashIds[i].serverId);
                        found = true;
                    } else if (found)
                    {
                        setOfReplicas.Add(storageHashIds[i].serverId);
                    }
                }

                return Shuffle(setOfReplicas);
            }

            public List<string> Shuffle(List<string> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    string value = list[k];
                    list[k] = list[n];
                    list[n] = value;
                }
                return list;
            }
        }

        public struct HashStorageNode
        {
            public string serverId;
            public int hashUpperBound;
        }
    }
}
