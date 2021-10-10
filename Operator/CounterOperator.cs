using System;
using System.Collections.Generic;
using System.Text;

namespace Operator
{
    class CounterOperator : IDIDAOperator
    {
        DIDAStorageNode[] storageReplicas;
        void IDIDAOperator.ConfigureStorage(DIDAStorageNode[] storageReplicas, delLocateStorageId locationFunction)
        {
            this.storageReplicas = storageReplicas;
        }

        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input)
        {
            // READ the value from storage node
            // var value = this.storageReplicas[0].
            // increment
            // UPDATE the value to storage
            throw new NotImplementedException();
        }
    }
}
