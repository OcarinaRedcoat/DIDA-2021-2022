using System;
using DIDAWorker;

namespace DIDAOperator
{

    public class SimpleOperator : IDIDAOperator
    {
        IDIDAStorage _storageProxy;

        // this operator increments the storage record identified in the metadata record every time it is called.
        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            _storageProxy.write(new DIDAWriteRequest { Id = input, Val = "2" });

            Console.WriteLine("simple write record:" + input);
            return "end";
        }

        void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy)
        {
            _storageProxy = storageProxy;
        }
    }

    public class UpdateAndChainOperator : IDIDAOperator
    {
        IDIDAStorage _storageProxy;

        // this operator increments the storage record identified in the metadata record every time it is called.
        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            var val = _storageProxy.updateIfValueIs(new DIDAUpdateIfRequest { Id = input, Newvalue = "1", Oldvalue = "1" });
            val = _storageProxy.updateIfValueIs(new DIDAUpdateIfRequest { Id = input, Newvalue = "1", Oldvalue = "1" });

            Console.WriteLine("[ UpdateAndChainOperator ] : updated record:" + input);
            return "end";
        }

        void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy)
        {
            _storageProxy = storageProxy;
        }
    }

    public class AddOperator : IDIDAOperator
    {
        IDIDAStorage _storageProxy;

        // this operator increments the storage record identified in the metadata record every time it is called.
        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            Console.WriteLine("[ AddOperator ] : Input string was: " + input);
            Console.Write("[ AddOperator ] : Reading data record: " + input + " with value: ");
            var val = _storageProxy.read(new DIDAReadRequest { Id = input, Version = new DIDAVersion { VersionNumber = -1, ReplicaId = -1 } });
            string storedString = val.Val;
            Console.WriteLine("[ AddOperator ] : Stored string -> " + storedString);
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
                output = "[ AddOperator - Error ] : int_conversion_failed";
                Console.WriteLine("[ AddOperator - Error ] : Operator expecting int but got chars: " + e.Message);
            }


            int oneAhead = Int32.Parse(input);
            oneAhead++;
            Console.Write("[ AddOperator ] : Reading data record: " + oneAhead + " with value: ");
            _storageProxy.read(new DIDAReadRequest { Id = oneAhead.ToString(), Version = new DIDAVersion { VersionNumber = -1, ReplicaId = -1 } });
            storedString = val.Val;
            Console.WriteLine("[ AddOperator ] : Stored string -> " + storedString);


            _storageProxy.write(new DIDAWriteRequest { Id = input, Val = output });
            Console.WriteLine("[ AddOperator ] : Writing data record:" + input + " with new value: " + output);
            return output;
        }

        void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy)
        {
            _storageProxy = storageProxy;
        }
    }
}

public class IncrementOperator : IDIDAOperator
{
    IDIDAStorage _storageProxy;

    public IncrementOperator()
    {
        AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
    }

    // this operator increments the storage record identified in the metadata record every time it is called.
    string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
    {
        Console.WriteLine("[ IncrementOperator ] : Input string was: " + input);
        Console.Write("[ IncrementOperator ] : Reading data record: " + meta.Id + " with value: ");
        var val = _storageProxy.read(new DIDAReadRequest { Id = meta.Id.ToString(), Version = new DIDAVersion { VersionNumber = -1, ReplicaId = -1 } });
        string storedString = val.Val;
        Console.WriteLine(storedString);
        int requestCounter = Int32.Parse(storedString);

        requestCounter++;
        //requestCounter += Int32.Parse(previousOperatorOutput);

        _storageProxy.write(new DIDAWriteRequest { Id = meta.Id.ToString(), Val = requestCounter.ToString() });
        Console.WriteLine("[ IncrementOperator ] : Writing data record:" + meta.Id + " with new value: " + requestCounter.ToString());
        return requestCounter.ToString();
    }

    void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy)
    {
        _storageProxy = storageProxy;
    }
}
