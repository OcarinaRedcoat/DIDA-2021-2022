using Grpc.Core;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNode
{
    class PuppetMasterStorageService : PMStorageService.PMStorageServiceBase
    {

        private StorageNodeLogic storageNode;
        public PuppetMasterStorageService(ref StorageNodeLogic logic) : base() {
            this.storageNode = logic; 
        }

  

        /*public override Task<PopulateReply> Populate(PopulateRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNode.Populate());
        }*/
    }

    class DatabaseServer
    {
        private int port;
        private string host;
        private string serverId;
        Server server;

        public DatabaseServer(string serverId, string host, int port, ref StorageNodeLogic logic)
        {
            this.serverId = serverId;
            this.host = host;
            this.port = port;

            server = new Server
            {
                Services = { PMStorageService.BindService(new PuppetMasterStorageService(ref logic)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Console.WriteLine("Starting Storage Node Server");
            Console.WriteLine("ServerId: " + this.serverId + " Host: " + this.host + " Port: " + this.port);
            server.Start();
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
