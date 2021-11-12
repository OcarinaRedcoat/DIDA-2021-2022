using System;
using System.Threading;
using DIDAWorker;

namespace ReadWriteConsistency
{
    public class IncrementOperator : IDIDAOperator
    {
        IDIDAStorage _storageProxy;

        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            Console.WriteLine("[ IncrementOperator ] : Input string was: " + input);
            Console.Write("[ IncrementOperator ] : Reading data record: " + input);
            var val = _storageProxy.read(new DIDAReadRequest { Id = input, Version = new DIDAVersion { VersionNumber = -1, ReplicaId = -1 } });
            string storedString = val.Val;
            Console.WriteLine("[ IncrementOperator ] : Read output: " + storedString);

            int requestCounter;
            string output;
            try
            {
                requestCounter = Int32.Parse(storedString);
                requestCounter++;
                output = requestCounter.ToString();
            }
            catch (Exception e)
            {
                output = "int_conversion_failed";
                Console.WriteLine("[ IncrementOperator ] : Operator expecting int but got chars: " + e.Message);
            }

            _storageProxy.write(new DIDAWriteRequest { Id = input, Val = output });
            Console.WriteLine("[ IncrementOperator ] : Writing data record:" + input + " with new value: " + output);
            return output;
        }

        void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy)
        {
            _storageProxy = storageProxy;
        }
    }

    public class WaitOperator : IDIDAOperator
    {
        public void ConfigureStorage(IDIDAStorage storageProxy)
        {
            return;
        }

        public string ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            Console.WriteLine("[ WaitOperator ] : Sleeping for 5 seconds.");
            Thread.Sleep(5000);
            return previousOperatorOutput;
        }
    }
}
