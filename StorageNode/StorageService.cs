using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNode
{
    class StorageNodeService : StorageService.StorageServiceBase
    {
        StorageNodeLogic snl;

        public StorageNodeService(ref StorageNodeLogic logic) : base()
        {
            this.snl = logic;
        }

        public ReadReply ReadSerialize(string id, DIDAVersion grpcVersionInput)
        {
            // Serialize Input
            DIDAStorage.DIDAVersion version = new DIDAStorage.DIDAVersion
            {
                replicaId = grpcVersionInput.ReplicaId,
                versionNumber = grpcVersionInput.VersionNumber
            };
            
            // Call logic operation
            DIDAStorage.DIDARecord record = snl.Read(id, version);

            // Serialize Output
            DIDAVersion grpcVersionOutput = new DIDAVersion
            {
                ReplicaId = record.version.replicaId,
                VersionNumber = record.version.versionNumber
            };

            DIDARecord grpcRecord = new DIDARecord
            {
                Id = record.id,
                Version = grpcVersionOutput,
                Val = record.val
            };
            return new ReadReply
            {
                Record = grpcRecord
            };
        }
        public override Task<ReadReply> Read(ReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(ReadSerialize(request.Id, request.Version));
        }


        public WriteReply WriteSerialize(string id, string value)
        {
            // Serialize Input

            // Call logic operation
            DIDAStorage.DIDAVersion version = snl.Write(id, value);

            // Serialize Output
            DIDAVersion grpcVersionOutput = new DIDAVersion
            {
                ReplicaId = version.replicaId,
                VersionNumber = version.versionNumber
            };

            return new WriteReply
            {
                Version = grpcVersionOutput
            };
        }

        public override Task<WriteReply> Write(WriteRequest request, ServerCallContext context)
        {
            return Task.FromResult(WriteSerialize(request.Id, request.Val));
        }

        public override Task<UpdateIfValueIsReply> UpdateIfValueIs(UpdateIfValueIsRequest request, ServerCallContext context)
        {
            snl.UpdateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
            return base.UpdateIfValueIs(request, context);
        }
    }

    class PuppetMasterStorageService : PMStorageService.PMStorageServiceBase
    {

        private StorageNodeLogic storageNode;

        public PuppetMasterStorageService(ref StorageNodeLogic logic) : base()
        {
            this.storageNode = logic;

        }



        public override Task<PopulateReply> Populate(PopulateRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNode.Populate(request.Data));
        }
    }


    class StorageServer
    {
        private int port;
        private string host;
        private string serverId;
        Server server;

        public StorageServer(string serverId, string host, int port, ref StorageNodeLogic logic)
        {
            this.serverId = serverId;
            this.host = host;
            this.port = port;

            server = new Server
            {
                Services = { StorageService.BindService(new StorageNodeService(ref logic)), PMStorageService.BindService(new PuppetMasterStorageService(ref logic)) } ,
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
