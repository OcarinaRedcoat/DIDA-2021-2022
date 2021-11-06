using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace WorkerNode
{
    class ConsistentHashing
    {

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
            return (int)(idx * Math.Floor((double)(RING_SIZE / numOfServers)));
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

            for (int i = 0; setOfReplicas.Count < replicationFactor; i = (i + 1) % storageHashIds.Count)
            {
                if (keyHash < storageHashIds[i].hashUpperBound && !found)
                {
                    setOfReplicas.Add(storageHashIds[i].serverId);
                    found = true;
                }
                else if (found)
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
