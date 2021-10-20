using Google.Protobuf.Collections;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Scheduler
{
    class WorkerManager
    {
        // TODO: Implements IDIDAStorage????

        private WorkerService.WorkerServiceClient client;

        private Dictionary<string, Hosts> workersHosts = new Dictionary<string, Hosts>();
        private Dictionary<string, GrpcChannel> workersChannels = new Dictionary<string, GrpcChannel>();
        private Dictionary<string, WorkerService.WorkerServiceClient> workersClients = new Dictionary<string, WorkerService.WorkerServiceClient>();

        public WorkerManager()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            //GrpcChannel channel = GrpcChannel.ForAddress(url);
            //client = new WorkerService.WorkerServiceClient(channel);
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

                Console.WriteLine("Before the try - calling ProcessOperator" + req);
                try
                {
                    Console.WriteLine("Before the grpc call");
                    


                    ProcessOperatorReply reply = workersClients["w1"].ProcessOperator(req);
                    Console.WriteLine("After the grpc call");
                    Console.WriteLine("Response: " + reply.Okay);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }



            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        public void AddWorker(string workerId, string workerURL)
        {

            Hosts hosts = parseUrl(workerURL);
            workersHosts.Add(workerId, hosts);
            Console.WriteLine("Added WorkerId: " + workerId + " Host: " + hosts.host + " Port: " + hosts.port);

            GrpcChannel channelAux = GrpcChannel.ForAddress(workerURL);
            var clientAux = new WorkerService.WorkerServiceClient(channelAux);
            workersChannels.Add(workerId, channelAux);
            workersClients.Add(workerId, clientAux);

        }

        public Hosts parseUrl(string url)
        {
            Hosts result;
            result.host = url.Split("//")[1].Split(":")[0];
            result.port = Int32.Parse(url.Split("//")[1].Split(":")[1]);
            return result;
        }

    }

    public struct Hosts
    {
        public string host;
        public int port;
    }
    public struct Balancer
    {
        public string workerId;
        public int nTask;
    }

}
