using DIDAWorker;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace WorkerNode
{
    class WorkerNodeLogic
    {
        private bool debugMode;
        private GrpcChannel logChannel;
        private LogServerService.LogServerServiceClient logClient;
        private string workerId;

        public WorkerNodeLogic(string serverId, int gossipDelay, bool debug, string logURL)
        {
            debugMode = debug;
            workerId = serverId;
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

        public string ProcessOperator(DIDARequest req)
        {
            string output = "";
            try
            {
                string className = req.chain[req.next].op.classname;

                DIDAWorker.IDIDAOperator _op = ExtractOperator(className);

                _op.ConfigureStorage(
                    new DIDAStorageNode[]
                    {
                        new DIDAStorageNode
                            {
                                host = GetLocalIPAddress(),
                                port = 3000,
                                serverId = "s1"
                            }
                    },
                    MyLocationFunction
                );

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
            return output;
        }

        private static DIDAStorageNode MyLocationFunction(string id, OperationType type)
        {
            // TODO: Implement
            // Hashing to choose Storage
            return new DIDAStorageNode
            {
                host = GetLocalIPAddress(),
                port = 3000,
                serverId = "s1"
            };
        }

        private DIDAWorker.IDIDAOperator ExtractOperator(string className)
        {
            try
            {
                string _dllNameTermination = ".dll";
                string _currWorkingDir = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Libs";

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

    }
}
