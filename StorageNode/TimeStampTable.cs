using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class TimeStampTable
    {
        // Last of the 5 Versions stored
        private Dictionary<string, DIDAStorage.DIDAVersion> replicaTimestamp = new Dictionary<string, DIDAStorage.DIDAVersion>();
        private Dictionary<int, Dictionary<string, DIDAStorage.DIDAVersion>> timeStampTable = new Dictionary<int, Dictionary<string, DIDAStorage.DIDAVersion>>();
        private int myReplicaId;

        public TimeStampTable(List<int> storagesReplicaId, int myReplicaId)
        {
            this.myReplicaId = myReplicaId;
            foreach (int replicaId in storagesReplicaId)
            {
                timeStampTable.Add(replicaId, new Dictionary<string, DIDAStorage.DIDAVersion>());
            }
        }

        public List<string> GetMyKeys()
        {
            List<string> myKeys = new List<string>();
            foreach (string key in this.replicaTimestamp.Keys)
            {
                myKeys.Add(key);
            }
            return myKeys;
        }

        public Dictionary<string, DIDAStorage.DIDAVersion> GetMyReplicaTimeStamp()
        {
            return replicaTimestamp;
        }

        public Dictionary<string, DIDAStorage.DIDAVersion> GetReplicaTimeStamp(int replicaId)
        {
            return timeStampTable[replicaId];
        }

        public void SetNewKey(int replicaId, string key, DIDAStorage.DIDAVersion version)
        {
            if (replicaId == this.myReplicaId)
            {
                replicaTimestamp.Add(key, version);
            }
            else
            {
                timeStampTable[replicaId].Add(key, version);
            }
        }

        public void UpdateVersion(int replicaId, string key, DIDAStorage.DIDAVersion version)
        {
            if (replicaId == this.myReplicaId)
            {
                if (replicaTimestamp.ContainsKey(key))
                {
                    replicaTimestamp[key] = version;
                }
                else
                {
                    SetNewKey(replicaId, key, version);
                }
            }
            else
            {
                if (timeStampTable[replicaId].ContainsKey(key))
                {
                    timeStampTable[replicaId][key] = version;
                }
                else
                {
                    SetNewKey(replicaId, key, version);
                }
            }
        }
    }
}
