using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterCLI
{
    class LogService : LogServerService.LogServerServiceBase
    {
        public LogService()
        {
        }

        public LogReply log(string workerId, string message)
        {
            Console.WriteLine("LOG: WorkerId(" + workerId + ") : " + message);
            return new LogReply
            {
                Okay = true
            };
        }

        public override Task<LogReply> Log(LogRequest request, ServerCallContext context)
        {
            return Task.FromResult(log(request.WorkerId, request.Message));
        }
    }
    class LogServer
    {
        private Int32 port = 10001;
        private string host = "localhost"; // TODO: Care with other machin

        Server server;
        
        public LogServer()
        {
            server = new Server
            {
                Services = { LogServerService.BindService(new LogService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
        }

        public string GetURL()
        {
            return "http://" + host + ":" + port;
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
