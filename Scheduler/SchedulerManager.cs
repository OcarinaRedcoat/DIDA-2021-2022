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
            Console.WriteLine(request.ServerId + " " + request.Url);
            wm.AddWorker(request.ServerId, request.Url);
            Console.WriteLine("Before ProcessOperator");
            //wm.ProcessOperator();

            return Task.FromResult(new AddWorkerNodeReply
            {
                Okay = true
            });
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
