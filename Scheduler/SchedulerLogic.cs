using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Scheduler
{
    class SchedulerLogic
    {
        private Dictionary<string, WorkerNodeStruct> workerNodes = new Dictionary<string, WorkerNodeStruct>();

        private int IDcounter = 0;
        public SchedulerLogic()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
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
                string workerUrl = "http://" + request.Chain[0].Host + ":" + request.Chain[0].Port;
                WorkerNodeStruct worker = workerNodes[workerUrl];
                worker.client.ProcessOperatorAsync(request);
            }
            catch (RpcException e)
            {
                Console.WriteLine("[ ERROR ] - ProcessOperatorAsync Request Status code: " + e.StatusCode);
                return new RunApplicationReply { Okay = false };
            }

            return new RunApplicationReply { Okay = true };
        }

        private IList<DIDAAssignment> LoadBalancer(IList<DIDAOperatorID> operators)
        {
            IList<DIDAAssignment> assignment = new List<DIDAAssignment>();

            List<string> urls = new List<string>(workerNodes.Keys);

            for ((int i, int j) = (0,0); i < operators.Count; j = ++i % workerNodes.Count)
            {
                assignment.Add(new DIDAAssignment
                {
                    OperatorId = operators[i],
                    Host = ExtractHostFromUrl(urls[j]),
                    Port = ExtractPortFromUrl(urls[j]),
                    Output = ""
                });
            }

            return assignment;
        }

        public AddWorkerNodeReply AddWorker(string workerId, string workerURL)
        {
            WorkerNodeStruct worker;
            worker.serverId = workerId;
            worker.host = ExtractHostFromUrl(workerURL);
            worker.port = ExtractPortFromUrl(workerURL);
            worker.channel = GrpcChannel.ForAddress(workerURL);
            worker.client = new WorkerService.WorkerServiceClient(worker.channel);

            lock (this)
            {
                workerNodes.TryAdd(worker.getUrl(), worker);
            }

            Console.WriteLine("[ LOG ] : Added WorkerId: " + workerId + " Host: " + worker.host + " Port: " + worker.port);

            return new AddWorkerNodeReply { Okay = true };
        }

        public string ExtractHostFromUrl(string url)
        {
            return url.Split("//")[1].Split(":")[0];
        }

        public int ExtractPortFromUrl(string url)
        {
            return Int32.Parse(url.Split("//")[1].Split(":")[1]);
        }

    }

    public struct Balancer
    {
        public string workerId;
        public int nTask;
    }
    public struct WorkerNodeStruct
    {
        public string host;
        public int port;
        public string serverId;
        public GrpcChannel channel;
        public WorkerService.WorkerServiceClient client;

        public string getUrl() { return "http://" + host + ":" + port; }
    }
}
