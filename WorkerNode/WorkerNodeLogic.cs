using DIDAWorker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace WorkerNode
{
    class WorkerNodeLogic
    {
        public void ProcessOperator(DIDARequest req)
        {

            try
            {
                string className = req.chain[req.next].op.classname;

                // ASync ??
                DIDAWorker.IDIDAOperator _op = extractOperator(className);

                _op.ConfigureStorage(new DIDAStorageNode[] { new DIDAStorageNode { host = "localhost", port = 3000, serverId = "s1" } }, MyLocationFunction);

                string output = _op.ProcessRecord(req.meta, req.input);

                Console.WriteLine("Finish: " + output);

                int next = ++req.next;

                // TODO: Send to next Node
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: ", e);
                // TODO: handle this case, warn PuppetMaster
            }
        }

        private static DIDAStorageNode MyLocationFunction(string id, OperationType type)
        {
            return new DIDAStorageNode { host = "localhost", port = 3000, serverId = "s1" };
        }

        private DIDAWorker.IDIDAOperator extractOperator(string className)
        {
            try
            {

                string _dllNameTermination = ".dll";
                string _currWorkingDir = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\WorkerNode\\Operators";


                foreach (string filename in Directory.EnumerateFiles(_currWorkingDir))
                {

                    if (filename.EndsWith(_dllNameTermination))
                    {

                        Assembly _dll = Assembly.LoadFrom(filename);

                        Type[] _typeList = _dll.GetTypes();

                        Console.WriteLine(_typeList.Length);
                        foreach (Type type in _typeList)
                        {

                            if (type.Name == className)
                            {
                                return (DIDAWorker.IDIDAOperator)Activator.CreateInstance(type);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Message: ", e);
                throw e;
            }
            throw new Exception("Type not found!");
        }
    }
}
