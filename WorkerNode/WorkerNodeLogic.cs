﻿using DIDAWorker;
using Google.Protobuf.Collections;
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
        private List<StorageNode> storagesNodes;

        public WorkerNodeLogic(string serverId, int gossipDelay, bool debug, string logURL)
        {
            debugMode = debug;
            workerId = serverId;
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

        public string ProcessOperator(DIDARequest req)
        {
            string output = "";
            try
            {
                string className = req.chain[req.next].op.classname;

                DIDAWorker.IDIDAOperator _op = ExtractOperator(className);

                DIDAMetaRecord meta = new DIDAMetaRecord { Id = req.meta.Id };

                _op.ConfigureStorage(new StorageProxy(
                    new DIDAStorageNode[] {
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3000, serverId = "s1" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3001, serverId = "1234567" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3002, serverId = "s3" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3003, serverId = "s4" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3004, serverId = "s5" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3005, serverId = "s6" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3006, serverId = "s7" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3007, serverId = "s8" },
                        new DIDAStorageNode { host = GetLocalIPAddress(), port = 3008, serverId = "s9" }
                    },
                    meta)
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

        public struct StorageNode
        {
            public string serverId;
            public string url;
        }
    }
}
