using Google.Protobuf.Collections;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Scheduler
{
    class WorkerManager
    {
        // TODO: Implements IDIDAStorage????
        private string url;
        private WorkerService.WorkerServiceClient client;
        public WorkerManager(string url)
        {
            this.url = url;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            client = new WorkerService.WorkerServiceClient(channel);
        }

        public void ProcessOperator()
        {
            try
            {
                Console.WriteLine("Creating dummy req");
                DIDAMetaRecord grpcMetaDummy = new DIDAMetaRecord
                {
                    Id = 1
                };

                DIDAOperatorID opID = new DIDAOperatorID
                {
                    Classname = "CounterOperator",
                    Order = 1
                };

                ProcessOperatorRequest req = new ProcessOperatorRequest
                {
                    Meta = grpcMetaDummy,
                    Input = "1",
                    Next = 0,
                    ChainSize = 1
                };

                req.Chain.Add(new DIDAAssignment
                {
                    OperatorId = opID,
                    Host = "localhost",
                    Port = 3001,
                    Output = ""
                });

                Console.WriteLine("Before calling ProcessOperator" + req);

                ProcessOperatorReply reply = client.ProcessOperator(req);
                Console.WriteLine("Response: " + reply.Okay);

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }
    }
}
