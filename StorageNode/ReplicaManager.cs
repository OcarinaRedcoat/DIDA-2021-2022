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
        private int MAX_VERSIONS;
        private Dictionary<string, List<DIDAStorage.DIDAVersion>> replicaTimestamp = new Dictionary<string, List<DIDAStorage.DIDAVersion>>();
        private Dictionary<int, Dictionary<string, List<DIDAStorage.DIDAVersion>>> timeStampTable = new Dictionary<int, Dictionary<string, List<DIDAStorage.DIDAVersion>>>();

        public ReplicaManager(int myReplicaId, int maxVersions)
        {
            this.myReplicaId = myReplicaId;
            this.MAX_VERSIONS = maxVersions;
        }

        public void AddStorageReplica(int replicaId)
        {
            timeStampTable.Add(replicaId, new Dictionary<string, List<DIDAStorage.DIDAVersion>>());
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

        public Dictionary<string, List<DIDAStorage.DIDAVersion>> GetReplicaTimeStamp(int replicaId)
        {
            if (replicaId == this.myReplicaId)
            {
                return replicaTimestamp;
            }
            return timeStampTable[replicaId];
        }

        public void CreateNewTimeStamp(int replicaId, string key, DIDAStorage.DIDAVersion version)
        {
            List<DIDAStorage.DIDAVersion> newTimestamp = new List<DIDAStorage.DIDAVersion>();
            newTimestamp.Add(version);
            if (replicaId == this.myReplicaId)
            {
                replicaTimestamp.Add(key, newTimestamp);
            }
            else
            {
                timeStampTable[replicaId].Add(key, newTimestamp);
            }
        }

        public void CreateNewEmptyTimeStamp(int replicaId, string key)
        {
            if (replicaId == this.myReplicaId)
            {
                replicaTimestamp.Add(key, new List<DIDAStorage.DIDAVersion>());
            }
            else
            {
                timeStampTable[replicaId].Add(key, new List<DIDAStorage.DIDAVersion>());
            }
        }

        public void AddTimeStamp(int replicaId, string key, DIDAStorage.DIDAVersion version)
        {
            bool inserted = false;
            if (replicaId == this.myReplicaId)
            {
                for (int i = 0; i < replicaTimestamp[key].Count; i++)
                {
                    if (this.IsVersionEqual(replicaTimestamp[key][i], version))
                    {
                        inserted = true;
                        break;
                    }
                    if (this.IsVersionBigger(replicaTimestamp[key][i], version))
                    {
                        replicaTimestamp[key].Insert(i, version);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    replicaTimestamp[key].Add(version);
                }

                if (replicaTimestamp[key].Count > MAX_VERSIONS)
                {
                    replicaTimestamp[key].RemoveAt(0);
                }

            }
            else
            {
                for (int i = 0; i < timeStampTable[replicaId][key].Count; i++)
                {
                    if (this.IsVersionEqual(replicaTimestamp[key][i], version))
                    {
                        inserted = true;
                        break;
                    }
                    if (this.IsVersionBigger(timeStampTable[replicaId][key][i], version))
                    {
                        timeStampTable[replicaId][key].Insert(i, version);
                        inserted = true;
                        break;
                    }
                }

                if (!inserted)
                {
                    timeStampTable[replicaId][key].Add(version);
                }

                if (timeStampTable[replicaId][key].Count > MAX_VERSIONS)
                {
                    timeStampTable[replicaId][key].RemoveAt(0);
                }
            }
        }

        public bool IsVersionEqual(DIDAStorage.DIDAVersion dIDAVersion, DIDAStorage.DIDAVersion version)
        {
            return dIDAVersion.replicaId == version.replicaId && dIDAVersion.versionNumber == version.versionNumber;
        }

        public void ReplaceTimeStamp(int replicaId, RepeatedField<TimeStamp> timeStamp)
        {
            Dictionary<string, List<DIDAStorage.DIDAVersion>> newReplicaTimestamp = new Dictionary<string, List<DIDAStorage.DIDAVersion>>();
            foreach (TimeStamp ts in timeStamp)
            {
                newReplicaTimestamp.Add(ts.Key, new List<DIDAStorage.DIDAVersion>());
                foreach (DIDAVersion grpcVersion in ts.Timestamp)
                {
                    DIDAStorage.DIDAVersion version = new DIDAStorage.DIDAVersion
                    {
                        replicaId = grpcVersion.ReplicaId,
                        versionNumber = grpcVersion.VersionNumber
                    };
                    newReplicaTimestamp[ts.Key].Add(version);
                }
            }
            timeStampTable[replicaId] = newReplicaTimestamp;
        }

        public bool IsVersionBigger(DIDAStorage.DIDAVersion myVersion, DIDAStorage.DIDAVersion otherVersion)
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

        public bool IsVersionBigger(DIDAVersion myVersion, DIDAVersion otherVersion)
        {
            if (myVersion.VersionNumber > otherVersion.VersionNumber)
            {
                return true;
            }
            if (myVersion.VersionNumber == otherVersion.VersionNumber && myVersion.ReplicaId > otherVersion.ReplicaId)
            {
                return true;
            }
            return false;
        }

        public List<DIDAStorage.DIDAVersion> ComputeGossipTimestampsDifferences(List<DIDAStorage.DIDAVersion> myTimestamp, List<DIDAStorage.DIDAVersion> otherTimestamp)
        {
            List<DIDAStorage.DIDAVersion> timestampsDifferences = new List<DIDAStorage.DIDAVersion>();
            foreach (DIDAStorage.DIDAVersion myVersion in myTimestamp)
            {
                if (!ContainsVersion(otherTimestamp, myVersion))
                {
                    timestampsDifferences.Add(myVersion);
                }
            }
            return timestampsDifferences;
        }

        public bool ContainsVersion(List<DIDAStorage.DIDAVersion> timestamp, DIDAStorage.DIDAVersion version)
        {
            foreach (DIDAStorage.DIDAVersion tsVersion in timestamp)
            {
                if (tsVersion.versionNumber == version.versionNumber && tsVersion.replicaId == version.replicaId)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
