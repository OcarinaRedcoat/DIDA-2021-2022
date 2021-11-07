using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class TimeStampTable
    {

        
        private Dictionary<int, Dictionary<string, DIDAStorage.DIDAVersion>> timeStampTable = new Dictionary<int, Dictionary<string, DIDAStorage.DIDAVersion>>();
        private Dictionary<string, List<DIDAStorage.DIDARecord>> updateLogs = new Dictionary<string, List<DIDAStorage.DIDARecord>>();
        private int myReplicaId;

        public TimeStampTable(List<int> storagesReplicaId, int myReplicaId)
        {
            this.myReplicaId = myReplicaId;
            foreach (int replicaId in storagesReplicaId)
            {
                timeStampTable.Add(replicaId, new Dictionary<string, DIDAStorage.DIDAVersion>());
            }
        }

        public Dictionary<string, DIDAStorage.DIDAVersion> GetMyReplicaTimeStamp()
        {
            return timeStampTable[myReplicaId];
        }

        public Dictionary<string, DIDAStorage.DIDAVersion> GetReplicaTimeStamp(int replicaId)
        {
            return timeStampTable[replicaId];
        }

        public void SetNewKey(int replicaId, string key, DIDAStorage.DIDAVersion version)
        {
            timeStampTable[replicaId].Add(key, version);

        }

        public void UpdateVersion(int replicaId, string key, DIDAStorage.DIDAVersion version)
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

        public void AddNewLog(string key, DIDAStorage.DIDARecord newRecord) 
        {
            if (!updateLogs.ContainsKey(key))
            {
                updateLogs.Add(key, new List<DIDAStorage.DIDARecord>());
            }
            updateLogs[key].Add(newRecord);
        }


    }
}
