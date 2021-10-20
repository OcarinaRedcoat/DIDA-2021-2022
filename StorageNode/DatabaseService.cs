using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNode
{
    class DBService : DatabaseService.DatabaseServiceBase
    {
        public DumpReply DumpSerialize()
        {
            // Call logic operation
            // Hot to access storage???

            // Serialize Output
            return new DumpReply
            {
                
            };
        }
        public override Task<DumpReply> Dump(DumpRequest request, ServerCallContext context)
        {
            return Task.FromResult(DumpSerialize());
        }

    }

    class DatabaseServer
    {
        private int port;
        private string host;
        private string serverId;
        Server server;

        public DatabaseServer(string serverId, string host, int port)
        {
            this.serverId = serverId;
            this.host = host;
            this.port = port;

            server = new Server
            {
                Services = { DatabaseService.BindService(new DBService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Console.WriteLine("Starting Database Server");
            Console.WriteLine("ServerId: " + this.serverId + " Host: " + this.host + " Port: " + this.port);
            server.Start();
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
