using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class ReplicaManager
    {
        private int replicaId;
        private int gossipDelay;

        private TimeStampTable timeStampTable;

        public ReplicaManager(int replicaId, int gossipDelay, )
        {
            this.replicaId = replicaId;
            this.gossipDelay = gossipDelay;

        }


    }
}
