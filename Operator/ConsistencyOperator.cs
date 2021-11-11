using System;
using System.Threading;
using DIDAWorker;

namespace DIDAOperator {

    public class WaitOperator : IDIDAOperator {

        //All this operator does is wait for 5 seconds, giving another application time to mess with the data consistency
        public void ConfigureStorage(IDIDAStorage storageProxy) {
            return;
        }

        public string ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput) {
            Thread.Sleep(5000);
            return previousOperatorOutput;
        }
    }

    public class IncrementOperator : IDIDAOperator {
        IDIDAStorage _storageProxy;

        // this operator increments the storage record identified in the metadata record every time it is called.
        // first prints it's input
        // then prints the value read
        // then if it's not a string it will print that it can't convert to string
        // finally it will increment the original input's value and write it down to storage

        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput) {
            Console.WriteLine("input string was: " + input);
            Console.Write("reading data record: " + input + " with value: ");
            var val = _storageProxy.read(new DIDAReadRequest { Id = input, Version = new DIDAVersion { VersionNumber = -1, ReplicaId = -1 } });
            string storedString = val.Val;
            Console.WriteLine(storedString);
            int requestCounter;
            string output;
            try {
                requestCounter = Int32.Parse(storedString);
                requestCounter++;
                output = requestCounter.ToString();
            } catch (Exception e) {
                output = "int_conversion_failed";
                Console.WriteLine(" operator expecting int but got chars: " + e.Message);
            }


            _storageProxy.write(new DIDAWriteRequest { Id = input, Val = output });
            Console.WriteLine("writing data record:" + input + " with new value: " + output);
            return output;
        }

        void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy) {
            _storageProxy = storageProxy;
            }
        }

}
