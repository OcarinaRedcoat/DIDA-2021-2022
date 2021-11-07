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
        private PCSLogic pcsLogic;

        public ProcessCreationService(ref PCSLogic pcsLogic)
        {
            this.pcsLogic = pcsLogic;
        }
        public override Task<CreateWorkerNodeReply> CreateWorkerNode(CreateWorkerNodeRequest request, ServerCallContext context)
        {
            return Task.FromResult(pcsLogic.CreateWorkerNode(request.ServerId, request.Url, request.Delay, request.Debug, request.LogURL));
        }

        public override Task<CreateStorageNodeReply> CreateStorageNode(CreateStorageNodeRequest request, ServerCallContext context)
        {
            return Task.FromResult(pcsLogic.CreateStorageNode(request.ServerId, request.Url, request.GossipDelay, request.ReplicaId));
        }

        public override Task<NukeReply> NukeStorage(NukeRequest request, ServerCallContext context)
        {
            return Task.FromResult(pcsLogic.NukeStorage(request.ServerId));
        }

        public override Task<NukeAllReply> Nuke(NukeAllRequest request, ServerCallContext context)
        {
            return Task.FromResult(pcsLogic.Nuke());
        }
    }


    class PCSServer
    {
        private Int32 port = 10000;
        private string host = GetLocalIPAddress();
        private PCSLogic pcsLogic;
        Server server;

        public PCSServer(ref PCSLogic pcsLogic)
        {
            this.pcsLogic = pcsLogic;
            server = new Server
            {
                Services = { PCSService.BindService(new ProcessCreationService(ref pcsLogic)) },
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
            pcsLogic.WaitForProcesses();
        }
    }

}
