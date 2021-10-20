using System;
using System.Collections.Generic;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;


namespace PuppetMasterCLI
{
    class PCSManager
    {

        private string url;
        private SchedulerService.SchedulerServiceClient schClient;

        private PCSService.PCSServiceClient client; // Sao varios PCS no final
//        private Dictionary<string, GrpcChannel> pcsChannels = new Dictionary<string, GrpcChannel>(); //TODO
//        private Dictionary<string, PCSService.PCSServiceClient> pcsClients = new Dictionary<string, PCSService.PCSServiceClient>(); //TODO

        public PCSManager(string url)
        {
            this.url = url;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            client = new PCSService.PCSServiceClient(channel);


            GrpcChannel channelScheduler = GrpcChannel.ForAddress("http://localhost:4000");
            schClient = new SchedulerService.SchedulerServiceClient(channelScheduler);
        }

        public void createWorkerNode(string serverId, string url, int gossipDelay) // return the Node
        {
            // GRPC call to PCS in order to create ...
            try
            {
                var reply = client.CreateWorkerNode(
                    new CreateWorkerNodeRequest
                    {
                        ServerId = serverId,
                        Url = url,
                        GossipDelay = gossipDelay
                    }
                );
                if (reply.Okay)
                {
                    Console.WriteLine("Okay");
                }
                else
                {
                    Console.WriteLine("Not Okay");
                }





                var request = new AddWorkerNodeRequest
                {
                    ServerId = serverId,
                    Url = url,
                };
                Console.WriteLine(request.ServerId + " " + request.Url);

                var replyScheduler = schClient.AddWorkerNode(request);
                if (replyScheduler.Okay)
                {
                    Console.WriteLine("Okay Scheduler");
                }
                else
                {
                    Console.WriteLine("Not Okay Scheduler");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void createStorageNode(string serverId, string url, int gossipDelay)
        {
            // GRPC call to PCS in order to create ...
            try
            {
                var reply = client.CreateStorageNode(
                    new CreateStorageNodeRequest
                    {
                        ServerId = serverId,
                        Url = url,
                        GossipDelay = gossipDelay
                    }
                );
                if (reply.Okay)
                {
                    Console.WriteLine("Okay");
                }
                else
                {
                    Console.WriteLine("Not Okay");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Crash(string serverId)
        {
            var reply = client.NukeStorage(
                new NukeRequest
                {
                    ServerId = serverId
                }
                );
            if (reply.Okay)
            {
                Console.WriteLine("Nuked Storage: " + serverId);
            }
            else
            {
                Console.WriteLine("Not Nuked Storage: " + serverId);
            }
        }
    }
}
