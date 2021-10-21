using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PCS
{
    class ProcessCreationService : PCSService.PCSServiceBase
    {

        public override Task<CreateWorkerNodeReply> CreateWorkerNode(CreateWorkerNodeRequest request, ServerCallContext context)
        {
            PCSLogic.CreateWorkerNode(request.ServerId, request.Url, request.GossipDelay, request.Debug, request.LogURL);
            return Task.FromResult(new CreateWorkerNodeReply
            {
                Okay = true
            });
        }

        public override Task<CreateStorageNodeReply> CreateStorageNode(CreateStorageNodeRequest request, ServerCallContext context)
        {
            PCSLogic.CreateStorageNode(request.ServerId, request.Url, request.GossipDelay, request.ReplicaId);
            return Task.FromResult(new CreateStorageNodeReply
            {
                Okay = true
            });
        }

        public override Task<NukeReply> NukeStorage(NukeRequest request, ServerCallContext context)
        {
            PCSLogic.NukeStorage(request.ServerId);
            return Task.FromResult(new NukeReply
            {
                Okay = true
            });
        }

        public override Task<NukeAllReply> Nuke(NukeAllRequest request, ServerCallContext context)
        {
            PCSLogic.Nuke();
            return Task.FromResult(new NukeAllReply{});
        }
    }


    class PCSServer
    {
        private Int32 port = 10000;
        private string host = GetLocalIPAddress();

        Server server;

        public PCSServer()
        {
            server = new Server
            {
                Services = { PCSService.BindService(new ProcessCreationService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
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

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
            PCSLogic.WaitForProcesses();
        }
    }

}
