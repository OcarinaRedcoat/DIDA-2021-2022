using DIDAWorker;
using Google.Protobuf.Collections;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace WorkerNode
{
    class WorkerNodeLogic
    {
        private bool debugMode;
        private GrpcChannel logChannel;
        private LogServerService.LogServerServiceClient logClient;
        private string workerId;
        private List<StorageNode> storagesNodes;
        private int responseDelay;

        public WorkerNodeLogic(string serverId, int delay, bool debug, string logURL)
        {
            debugMode = debug;
            workerId = serverId;
            responseDelay = delay;
            storagesNodes = new List<StorageNode>();
            if (debugMode)
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                logChannel = GrpcChannel.ForAddress(logURL);
                logClient = new LogServerService.LogServerServiceClient(logChannel);
            }
        }

        public StatusReply Status()
        {
            Console.WriteLine("This is my Status: " + workerId);
            // TODO: More info
            return new StatusReply { };
        }

        public string ProcessOperator(DIDARequest req, DIDAMetaRecord prevMeta, out DIDAMetaRecord newMeta)
        {
            string output = "";

            DIDAStorageNode[] storageNodesArray = new DIDAStorageNode[storagesNodes.Count];

            for (int i = 0; i < storagesNodes.Count; i++)
            {
                storageNodesArray[i] = new DIDAStorageNode { host = ParseHost(storagesNodes[i].url), port = ParsePort(storagesNodes[i].url), serverId = storagesNodes[i].serverId };
            }

            DIDAMetaRecord meta = prevMeta;

            StorageProxy proxy = new StorageProxy(storageNodesArray, meta);

            try
            {
                string className = req.chain[req.next].op.classname;

                DIDAWorker.IDIDAOperator _op = ExtractOperator(className);

                _op.ConfigureStorage(proxy);

                string previouOutput = req.next == 0 ? "" : req.chain[req.next - 1].output;

                output = _op.ProcessRecord(req.meta, req.input, previouOutput);

                // TODO: If is the last operator from the chain ping scheduler

                if (debugMode)
                {
                    logClient.Log(new LogRequest
                    {
                        WorkerId = workerId,
                        Message = output
                    });
                }
                Console.WriteLine("Output from Operator: " + output);

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: ", e);
                // TODO: handle this case, warn PuppetMaster
            }

            newMeta = proxy.GetMetaRecord();

            Thread.Sleep(responseDelay);
            return output;
        }

        private DIDAWorker.IDIDAOperator ExtractOperator(string className)
        {
            try
            {
                string _dllNameTermination = ".dll";
                string _currWorkingDir = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Libs\\netcoreapp3.1\\";

                foreach (string filename in Directory.EnumerateFiles(_currWorkingDir))
                {

                    if (filename.EndsWith(_dllNameTermination))
                    {

                        Assembly _dll = Assembly.LoadFrom(filename);

                        Type[] _typeList = _dll.GetTypes();

                        foreach (Type type in _typeList)
                        {
                            if (type.Name == className)
                            {
                                return (DIDAWorker.IDIDAOperator) Activator.CreateInstance(type);
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

        public SetupReply SetupStorage(RepeatedField<StorageInfo> storages)
        {
            foreach (StorageInfo s in storages)
            {
                storagesNodes.Add(new StorageNode
                {
                    serverId = s.Id,
                    url = s.Url
                });
                Console.WriteLine("Receive new Storage " + s.Id);
            }
            return new SetupReply
            {
                Okay = true
            };
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public string ParseHost(string url)
        {
            return url.Split("//")[1].Split(":")[0];
  
        }
        public int ParsePort(string url)
        {
            return Int32.Parse(url.Split("//")[1].Split(":")[1]);
        }

        public struct StorageNode
        {
            public string serverId;
            public string url;
        }
    }
}
