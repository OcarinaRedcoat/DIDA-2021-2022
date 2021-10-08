using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PuppetMasterCLI
{
    class LogServer
    {
        private Int32 port = 10001;
        private string host = "localhost";

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

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }

    class LogService : LogServerService.LogServerServiceBase
    {
        public LogService()
        {
        }
    }
}
