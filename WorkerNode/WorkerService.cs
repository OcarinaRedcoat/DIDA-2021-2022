using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkerNode
{
    class WorkerNodeService : WorkerService.WorkerServiceBase
    {

        public override Task<ProcessOperatorReply> ProcessOperator(ProcessOperatorRequest request, ServerCallContext context)
        {
            return base.ProcessOperator(request, context);
        }
    }

    class WorkerServer
    {
        private int port;
        private string host;
        private string serverId;
        Server server;

        public WorkerServer(string serverId, string host, int port)
        {
            this.serverId = serverId;
            this.host = host;
            this.port = port;

            server = new Server
            {
                Services = { WorkerService.BindService(new WorkerNodeService()) },
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
