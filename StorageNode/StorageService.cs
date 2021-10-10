using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNode
{
    class StorageNodeService : StorageService.StorageServiceBase
    {
        StorageNodeLogic snl = new StorageNodeLogic();
        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(snl.Read(request.Id, request.Version));
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            var didaVersion = snl.Write(request.Id, request.Val);
            return Task.FromResult(
                new WriteReply
                {
                    
                }
 )
        }

        public override Task<UpdateIfValueIsReply> UpdateIfValueIs(UpdateIfValueIsRequest request, ServerCallContext context)
        {
            snl.UpdateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
            return base.UpdateIfValueIs(request, context);
        }
    }

    class StorageServer
    {
        private Int32 port;
        private string host;
        private string serverId;
        Server server;

        public StorageServer(string serverId, string host, int port)
        {
            this.serverId = serverId;
            this.host = host;
            this.port = port;

            server = new Server
            {
                Services = { StorageService.BindService(new StorageNodeService()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Console.WriteLine("Starting Storage Node");
            Console.WriteLine("ServerId: " + this.serverId + " Host: " + this.host + " Port: " + this.port);
            server.Start();
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
            // PCSLogic.WaitForProcesses();
        }
    }
}
