using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class ReplicaManager
    {
        private int replicaId;
        private int gossipDelay;
        private List<int> replicaTimestamp = new List<int>();
        private List<List<int>> timestampTable = new List<List<int>>();

        public ReplicaManager(int replicaId, int gossipDelay)
        {
            this.replicaId = replicaId;
            this.gossipDelay = gossipDelay;
        }


    }
}
