using Google.Protobuf.Collections;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WorkerNode
{
    class WorkerNodeService : WorkerService.WorkerServiceBase
    {

        WorkerNodeLogic wnl = new WorkerNodeLogic();

        public ProcessOperatorReply ProcessOperatorSerialize(DIDAMetaRecord grpcMeta, string grpcInput, int grpcNext, int grpcChainSize, RepeatedField<DIDAAssignment> grpcChain)
        {
            // Serialize Input
            DIDAWorker.DIDAMetaRecord meta = new DIDAWorker.DIDAMetaRecord
            {
                id = grpcMeta.Id
            };

            DIDAWorker.DIDAAssignment[] chain = new DIDAWorker.DIDAAssignment[grpcChainSize];

            DIDAWorker.DIDAOperatorID auxOperatorId;

            for (int i = 0; i < grpcChainSize; i++)
            {
                auxOperatorId = new DIDAWorker.DIDAOperatorID
                {
                    classname = grpcChain[i].OperatorId.Classname,
                    order = grpcChain[i].OperatorId.Order
                };

                chain[i] = new DIDAWorker.DIDAAssignment
                {
                    op = auxOperatorId,
                    host = grpcChain[i].Host,
                    port = grpcChain[i].Port,
                    output = grpcChain[i].Output
                };
            }

            DIDAWorker.DIDARequest req = new DIDAWorker.DIDARequest
            {
                meta = meta,
                input = grpcInput,
                next = grpcNext,
                chainSize = grpcChainSize,
                chain = chain
            };

            // Call logic operation
            wnl.ProcessOperator(req);

            // Serialize Output
            return new ProcessOperatorReply
            {
                Okay = true
            };
        }

        public override Task<ProcessOperatorReply> ProcessOperator(ProcessOperatorRequest request, ServerCallContext context)
        {
            return Task.FromResult(ProcessOperatorSerialize(request.Meta, request.Input, request.Next, request.ChainSize, request.Chain));
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
