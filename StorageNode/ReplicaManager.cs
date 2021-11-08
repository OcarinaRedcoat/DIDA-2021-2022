using System;
using System.Collections.Generic;
using System.Text;
using CHashing;
using Google.Protobuf.Collections;

namespace StorageNode
{
    class ReplicaManager
    {
        private int myReplicaId;
        private int gossipDelay;
        private List<int> replicaIds = new List<int>();
        Dictionary<string, StorageNodeStruct> storageÑodes = new Dictionary<string, StorageNodeStruct>();
        private ConsistentHashing consistentHashing;
        private TimeStampTable timeStampTable;
        private Dictionary<string, List<DIDAStorage.DIDARecord>> updateLogs = new Dictionary<string, List<DIDAStorage.DIDARecord>>();

        public ReplicaManager(int myReplicaId, int gossipDelay, List<StorageNodeStruct> storageNodes)
        {
            this.myReplicaId = myReplicaId;
            this.gossipDelay = gossipDelay; // TODO: Create timer to call the Gossip function
            List<string> storageKeysList = new List<string>();
            foreach (StorageNodeStruct sns in storageNodes)
            {
                this.replicaIds.Add(sns.replicaId);
                this.storageÑodes.Add(sns.serverId, sns);
                storageKeysList.Add(sns.serverId);
            }
            this.consistentHashing = new ConsistentHashing(storageKeysList);
            this.timeStampTable = new TimeStampTable(replicaIds, this.myReplicaId);
        }

        public Dictionary<string, DIDAStorage.DIDAVersion> GetReplicaTimeStamp(int replicaId)
        {
            return this.timeStampTable.GetReplicaTimeStamp(replicaId);
        }

        public Dictionary<string, DIDAStorage.DIDAVersion> GetMyReplicaTimeStamp()
        {
            return this.timeStampTable.GetMyReplicaTimeStamp();
        }

        public void AddNewLog(string key, DIDAStorage.DIDARecord newRecord)
        {
            if (!updateLogs.ContainsKey(key))
            {
                updateLogs.Add(key, new List<DIDAStorage.DIDARecord>());
            }
            updateLogs[key].Add(newRecord);
        }

        /*
        public void Gossip()
        {
            List<string> keys = this.timeStampTable.GetMyKeys();
            Dictionary<string, DIDAStorage.DIDAVersion> myTimestamp = this.timeStampTable.GetMyReplicaTimeStamp();
            Dictionary<string, GossipRequest> gossipRequests = new Dictionary<string, GossipRequest>();

            foreach (string key in keys)
            {
                List<string> setOfReplicas = this.consistentHashing.ComputeSetOfReplicas(key);
                foreach (string serverId in setOfReplicas)
                {
                    if (!gossipRequests.ContainsKey(serverId))
                    {
                        gossipRequests.Add(serverId, new GossipRequest { ReplicaId = myReplicaId });
                    }
                    StorageNodeStruct sns = storageÑodes[serverId];
                    Dictionary<string, DIDAStorage.DIDAVersion> snsTimestamp = this.timeStampTable.GetReplicaTimeStamp(sns.replicaId);
                    if (VersionIsBiggerComparator(myTimestamp[key], snsTimestamp[key]))
                    {
                        // Add to request
                        // gossipRequests[serverId].ReplicaTimestamp.Add(new DIDARecord
                        // {
                        //     Id = key,
                        //     Val = 
                        // });
                    }
                }
            }
        }
        */

        public bool VersionIsBiggerComparator(DIDAStorage.DIDAVersion myVersion, DIDAStorage.DIDAVersion otherVersion)
        {
            if (myVersion.versionNumber > otherVersion.versionNumber)
            {
                return true;
            }
            if (myVersion.versionNumber == otherVersion.versionNumber && myVersion.replicaId > otherVersion.replicaId)
            {
                return true;
            }
            return false;
        }

        public void UpdateTimeStamp(int replicaId, string key, DIDAStorage.DIDAVersion version)
        {
            this.timeStampTable.UpdateVersion(replicaId, key, version);
        }

        public void AddNewKeyTimeStamp(string key, DIDAStorage.DIDAVersion version)
        {
            this.timeStampTable.SetNewKey(myReplicaId, key, version);
        }

        public void UpdateReplicaTimeStamp(int replicaId, RepeatedField<TimeStamp> replicaTimestamp)
        {
            foreach (TimeStamp ts in replicaTimestamp)
            {
                this.timeStampTable.UpdateVersion(replicaId, ts.Key, new DIDAStorage.DIDAVersion
                {
                    versionNumber = ts.ReplicaTimestamp.VersionNumber,
                    replicaId = ts.ReplicaTimestamp.ReplicaId
                });
            }
        }

        public List<string> GetMyKeys()
        {
            return this.timeStampTable.GetMyKeys();
        }

        public List<string> ComputeSetOfReplicas(string key)
        {
            return this.consistentHashing.ComputeSetOfReplicas(key);
        }
    }
}
