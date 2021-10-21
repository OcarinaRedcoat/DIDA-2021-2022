using System;
using System.Collections.Generic;
using System.Text;
using DIDAWorker;

namespace Operator
{
    class CounterOperator : IDIDAOperator
    {
        DIDAStorageNode[] storageReplicas;
        void IDIDAOperator.ConfigureStorage(DIDAStorageNode[] storageReplicas, delLocateStorageId locationFunction)
        {
            this.storageReplicas = storageReplicas;
        }

        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            // READ the value from storage node
            // var value = this.storageReplicas[0].
            // increment
            // UPDATE the value to storage
            int intInput;
            if (previousOperatorOutput.Length == 0)
                intInput = Int32.Parse(input);
            else
                intInput = Int32.Parse(previousOperatorOutput);

            int intOutput = ++intInput;
            
            string output = intOutput.ToString();

            return output;
        }
    }
}
