using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{

    class SchService : SchedulerService.SchedulerServiceBase
    {
        WorkerManager wm = new WorkerManager();
        public override Task<AddWorkerNodeReply> AddWorkerNode(AddWorkerNodeRequest request, ServerCallContext context)
        {
            return Task.FromResult(wm.AddWorker(request.ServerId, request.Url));
        }

        public override Task<RunApplicationReply> RunApplication(RunApplicationRequest request, ServerCallContext context)
        {
            return Task.FromResult(wm.RunApplication(request.Chain, request.Input, request.ChainSize));
        }

    }

    class SchedulerServer
    {
        private int port;
        private string host;
        private string serverId;
        Server server;

        public SchedulerServer(string serverId, string host, int port)
        {
            this.serverId = serverId;
            this.host = host;
            this.port = port;

            server = new Server
            {
                Services = { SchedulerService.BindService(new SchService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Console.WriteLine("Starting Worker Node Server");
            Console.WriteLine("ServerId: " + this.serverId + " Host: " + this.host + " Port: " + this.port);
            server.Start();
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
