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
            Console.WriteLine("Inside ProcessOperator: ");

            try
            {
                string className = req.chain[req.next].op.classname;
                Console.WriteLine("B " + className);
                // ASync ??
                DIDAWorker.IDIDAOperator _op = extractOperator(className);
                Console.WriteLine("C");
                _op.ConfigureStorage(new DIDAStorageNode[] { new DIDAStorageNode { host = "localhost", port = 3000, serverId = "s1" } }, MyLocationFunction);
                Console.WriteLine("D");
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
                Console.WriteLine("Z");
                string _dllNameTermination = ".dll";
                string _currWorkingDir = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\WorkerNode\\Operators";

                Console.WriteLine("Current: ");
                Console.WriteLine(_currWorkingDir);

                foreach (string filename in Directory.EnumerateFiles(_currWorkingDir))
                {
                    Console.WriteLine("file in cwd: " + filename);
                    if (filename.EndsWith(_dllNameTermination))
                    {
                        Console.WriteLine("File is a dll...Let's look at it's contained types...");
                        Assembly _dll = Assembly.LoadFrom(filename);
                        Console.WriteLine("After Assembly");
                        Type[] _typeList = _dll.GetTypes();
                        Console.WriteLine("After GetTypes ");
                        Console.WriteLine("After GetTypes ");
                        Console.WriteLine(_typeList.Length);
                        foreach (Type type in _typeList)
                        {
                            Console.WriteLine("type contained in dll: " + type.Name);
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
