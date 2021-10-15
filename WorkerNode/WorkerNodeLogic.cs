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
        internal void ProcessOperator(DIDARequest req)
        {
            string className = req.chain[req.chainSize].op.classname;

            try
            {
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
            string _dllNameTermination = ".dll";
            string _currWorkingDir = Directory.GetCurrentDirectory() + "..\\..\\..\\Operators";

            foreach (string filename in Directory.EnumerateFiles(_currWorkingDir))
            {
                Console.WriteLine("file in cwd: " + filename);
                if (filename.EndsWith(_dllNameTermination))
                {
                    Console.WriteLine("File is a dll...Let's look at it's contained types...");
                    Assembly _dll = Assembly.LoadFrom(filename);
                    Type[] _typeList = _dll.GetTypes();
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
            throw new Exception("Type not found!");
        }
    }
}
