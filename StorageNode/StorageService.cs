using Google.Protobuf.Collections;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageNode
{
    class StorageNodeService : DIDAStorageService.DIDAStorageServiceBase
    {
        StorageNodeLogic storageNodeLogic;

        public StorageNodeService(ref StorageNodeLogic logic) : base()
        {
            this.storageNodeLogic = logic;
        }

        public DIDARecordReply ReadSerialize(string id, DIDAVersion grpcVersionInput)
        {
            // Serialize Input
            DIDAStorage.DIDAVersion version = new DIDAStorage.DIDAVersion
            {
                replicaId = grpcVersionInput.ReplicaId,
                versionNumber = grpcVersionInput.VersionNumber
            };
            
            // Call logic operation
            DIDAStorage.DIDARecord record = storageNodeLogic.Read(id, version);

            // Serialize Output
            DIDAVersion grpcVersionOutput = new DIDAVersion
            {
                ReplicaId = record.version.replicaId,
                VersionNumber = record.version.versionNumber
            };

            return new DIDARecordReply
            {
                Id = record.id,
                Version = grpcVersionOutput,
                Val = record.val
            };
        }

        public override Task<DIDARecordReply> read(DIDAReadRequest request, ServerCallContext context)
        {
            return Task.FromResult(ReadSerialize(request.Id, request.Version));
        }

        public DIDAVersion WriteSerialize(string id, string value)
        {
            // Serialize Input

            // Call logic operation
            DIDAStorage.DIDAVersion version = storageNodeLogic.Write(id, value);

            // Serialize Output
            return new DIDAVersion
            {
                ReplicaId = version.replicaId,
                VersionNumber = version.versionNumber
            };
        }

        public override Task<DIDAVersion> write(DIDAWriteRequest request, ServerCallContext context)
        {
            return Task.FromResult(WriteSerialize(request.Id, request.Val));
        }

        public override Task<DIDAVersion> updateIfValueIs(DIDAUpdateIfRequest request, ServerCallContext context)
        {
            return  storageNodeLogic.UpdateIfValueIs(request.Id, request.Oldvalue, request.Newvalue);
                
        }

    }

    class PuppetMasterStorageService : PMStorageService.PMStorageServiceBase
    {

        private StorageNodeLogic storageNode;

        public PuppetMasterStorageService(ref StorageNodeLogic logic) : base()
        {
            this.storageNode = logic;

        }

        public DumpReply DumpSerialize()
        {
            // Call logic operation
            var data = storageNode.Dump();

            // Serialize Output
            DumpReply reply = new DumpReply { };

            foreach (DIDAStorage.DIDARecord rec in data)
            {
                var grpcVersion = new DIDAVersion
                {
                    VersionNumber = rec.version.versionNumber,
                    ReplicaId = rec.version.replicaId
                };
                reply.Data.Add(new DIDARecord
                {
                    Id = rec.id,
                    Version = grpcVersion,
                    Val = rec.val
                });
            }

            return reply;
        }

        public override Task<DumpReply> Dump(DumpRequest request, ServerCallContext context)
        {
            return Task.FromResult(DumpSerialize());
        }

        public override Task<PopulateReply> Populate(PopulateRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNode.Populate(request.Data));
        }

        public override Task<AddStorageReply> AddStorage(AddStorageRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNode.AddStorage(request.Storages));
        }
    }

    class StorageStatusService : StatusService.StatusServiceBase
    {
        private StorageNodeLogic storageNodeLogic;

        public StorageStatusService(ref StorageNodeLogic logic)
        {
            this.storageNodeLogic = logic;
        }

        public override Task<StatusReply> Status(StatusRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNodeLogic.Status());
        }
    }

    class StorageGossipService : GossipService.GossipServiceBase
    {
        private StorageNodeLogic storageNodeLogic;

        public StorageGossipService(ref StorageNodeLogic logic)
        {
            this.storageNodeLogic = logic;
        }

        public override Task<GossipReply> Gossip(GossipRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNodeLogic.ReceiveGossip(request));
        }
    }

    class StorageUpdateIfValueIsService : UpdateIfValueIsService.UpdateIfValueIsServiceBase
    {
        private StorageNodeLogic storageNodeLogic;

        public StorageUpdateIfValueIsService(ref StorageNodeLogic logic)
        {
            this.storageNodeLogic = logic;
        }

        public override Task<CommitPhaseReply> CommitPhase(CommitPhaseRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNodeLogic.CommitPhase(request));
        }

        public override Task<LockAndPullReply> LockAndPull(LockAndPullRequest request, ServerCallContext context)
        {
            return Task.FromResult(storageNodeLogic.LockAndPull(request));
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
                Services =
                {
                    DIDAStorageService.BindService(new StorageNodeService(ref logic)),
                    PMStorageService.BindService(new PuppetMasterStorageService(ref logic)),
                    StatusService.BindService(new StorageStatusService(ref logic)),
                    GossipService.BindService(new StorageGossipService(ref logic)),
                    UpdateIfValueIsService.BindService(new StorageUpdateIfValueIsService(ref logic))
                } ,
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Console.WriteLine("[ LOG ] : Starting Storage Node Server");
            Console.WriteLine("[ LOG ] : ServerId: " + this.serverId + " Host: " + this.host + " Port: " + this.port);
            server.Start();
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
