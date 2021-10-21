using Google.Protobuf.Collections;
using Grpc.Core;
using System;
using Grpc.Net.Client;
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


            Console.WriteLine("Calling ProcessOperator Logic: ", req);
            // Call logic operation

            req.chain[req.next].output = wnl.ProcessOperator(req);

            req.next++;

            Console.WriteLine("Next: " + req.next);
            foreach (DIDAWorker.DIDAAssignment ch in req.chain)
            {
                Console.WriteLine("Chain Host: " + ch.host + " Port: " + ch.port + " Operator: " + ch.op.classname + " Output: " + ch.output);
            }

            if (req.next < req.chainSize)
            {
                string url = "http://" +  req.chain[req.next].host + ":" + req.chain[req.next].port;
                Console.WriteLine("URL Next: " + url);
                GrpcChannel channelAux = GrpcChannel.ForAddress(url);
                Console.WriteLine("A");
                var client = new WorkerService.WorkerServiceClient(channelAux);
                Console.WriteLine("B");
                Console.WriteLine(client.ToString());
                ProcessOperatorRequest newReq = GenerateRequest(req);
                Console.WriteLine("C");
                Console.WriteLine(req.ToString());
                client.ProcessOperatorAsync(newReq); //ASYNC
            }

            // Serialize Output
            return new ProcessOperatorReply
            {
                Okay = true
            };
        }

        public ProcessOperatorRequest GenerateRequest(DIDAWorker.DIDARequest req)
        {
            DIDAMetaRecord metaRecord = new DIDAMetaRecord
            {
                Id = req.meta.id
            };

            ProcessOperatorRequest newReq = new ProcessOperatorRequest
            {
                Meta = metaRecord,
                Input = req.input,
                Next = req.next,
                ChainSize = req.chainSize
            };

            foreach(DIDAWorker.DIDAAssignment assignment in req.chain)
            {
                DIDAOperatorID grpcOpId = new DIDAOperatorID
                {
                    Classname = assignment.op.classname,
                    Order = assignment.op.order
                };
                DIDAAssignment grpcAssignment = new DIDAAssignment
                {
                    OperatorId = grpcOpId,
                    Host = assignment.host,
                    Port = assignment.port,
                    Output = assignment.output
                };
                newReq.Chain.Add(grpcAssignment);
            }

            return newReq;
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
