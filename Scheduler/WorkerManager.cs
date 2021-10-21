using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Scheduler
{
    class WorkerManager
    {
        // TODO: Implements IDIDAStorage????

        private WorkerService.WorkerServiceClient client;

        // url+port to id FIXME maybe not necessary
        private Dictionary<string, string> urlToID = new Dictionary<string, string>();

        private Dictionary<string, Hosts> workersHosts = new Dictionary<string, Hosts>();
        private Dictionary<string, GrpcChannel> workersChannels = new Dictionary<string, GrpcChannel>();
        private Dictionary<string, WorkerService.WorkerServiceClient> workersClients = new Dictionary<string, WorkerService.WorkerServiceClient>();

        private int IDcounter = 0;
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

        public RunApplicationReply RunApplication(IList<DIDAOperatorID> operators, string input, int chainSize)
        {
            IList<DIDAAssignment> assignment = LoadBalancer(operators);

            // DIDARequest
            ProcessOperatorRequest request = new ProcessOperatorRequest
            {
                Meta = new DIDAMetaRecord
                {
                    Id = Interlocked.Increment(ref IDcounter)
                },
                Input = input,
                Next = 0,
                ChainSize = chainSize
            };
            request.Chain.Add(assignment);


            // Send DIDARequest to first worker 
            try
            {
                ProcessOperatorReply reply =  workersClients[urlToID[request.Chain[0].Host + request.Chain[0].Port]].ProcessOperator(request);

            } 
            catch ( Exception e)
            {
                Console.WriteLine(e.Message);
                return new RunApplicationReply { Okay = false };
            }

            return new RunApplicationReply { Okay = true };
        }

        private IList<DIDAAssignment> LoadBalancer(IList<DIDAOperatorID> operators)
        {
            //FIXME dumb algo just a circle
            IList<DIDAAssignment> assignment = new List<DIDAAssignment>();

            List<Hosts> hosts = new List<Hosts>(workersHosts.Values);

            for ((int i, int j) = (0,0); i < operators.Count; j = ++i % workersHosts.Count)
            {
                assignment.Add(new DIDAAssignment
                {
                    OperatorId = operators[i],
                    Host = hosts[j].host,
                    Port =hosts[j].port,
                    Output = ""
                });
            }
            Console.WriteLine(assignment.ToString());


            return assignment;
        }

        public AddWorkerNodeReply AddWorker(string workerId, string workerURL)
        {
            //TODO lock this mofo

            Console.WriteLine(workerId + " " + workerURL);

            Hosts hosts = parseUrl(workerURL);

            //FIXME maybe not necessary
            urlToID.Add(hosts.host + hosts.port, workerId);

            workersHosts.Add(workerId, hosts);
            Console.WriteLine("Added WorkerId: " + workerId + " Host: " + hosts.host + " Port: " + hosts.port);

            GrpcChannel channelAux = GrpcChannel.ForAddress(workerURL);
            var clientAux = new WorkerService.WorkerServiceClient(channelAux);
            workersChannels.Add(workerId, channelAux);
            workersClients.Add(workerId, clientAux);

            return new AddWorkerNodeReply { Okay = true };

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
