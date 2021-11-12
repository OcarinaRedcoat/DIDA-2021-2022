using System;
using DIDAWorker;

namespace UpdateTiebreak
{
    public class LoginOperator : IDIDAOperator
    {
        IDIDAStorage _storageProxy;

        string IDIDAOperator.ProcessRecord(DIDAMetaRecord meta, string input, string previousOperatorOutput)
        {
            Console.WriteLine("[ LoginOperator ] : User name (input): " + input);
            Console.Write("[ LoginOperator ] : Trying to login... (UpdateIfValueIs)");
            var val = _storageProxy.updateIfValueIs(new DIDAUpdateIfRequest { Id = input, Newvalue = "in", Oldvalue = "out" });

            Console.WriteLine("[ LoginOperator ] : User " + input + " logged in!");

            return input + " logged in!";
        }

        void IDIDAOperator.ConfigureStorage(IDIDAStorage storageProxy)
        {
            _storageProxy = storageProxy;
        }
    }
}
