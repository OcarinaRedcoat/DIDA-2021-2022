using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;


namespace PuppetMasterCLI
{
    class PCSManager
    {

        private string url;
        private PCSService.PCSServiceClient client;

        private string schedulerUrl;
        private SchedulerService.SchedulerServiceClient schClient;

        public PCSManager(string url)
        {
            this.url = url;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            client = new PCSService.PCSServiceClient(channel);
            GrpcChannel channelScheduler = GrpcChannel.ForAddress("http://" + GetLocalIPAddress() + ":4000");
            schClient = new SchedulerService.SchedulerServiceClient(channelScheduler);
        }

        public void SetScheduler(string schedulerUrl)
        {
            this.schedulerUrl = schedulerUrl;
            GrpcChannel channelScheduler = GrpcChannel.ForAddress(schedulerUrl);
            schClient = new SchedulerService.SchedulerServiceClient(channelScheduler);
        }

        public void createWorkerNode(string serverId, string url, int delay, bool debug, string logURL)
        {
            try
            {
                var reply = client.CreateWorkerNode(
                    new CreateWorkerNodeRequest
                    {
                        ServerId = serverId,
                        Url = url,
                        Delay = delay,
                        Debug = debug,
                        LogURL = logURL
                    }
                );
                if (reply.Okay)
                {
                    Console.WriteLine("[ LOG ] : Created worker node!");
                }
                else
                {
                    Console.WriteLine("[ ERROR ] : Error creating Worker node!");
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
                    Console.WriteLine("[ LOG ] : Added worker to Scheduler");
                }
                else
                {
                    Console.WriteLine("[ ERROR ] : Error trying to add worker to Scheduler");
                }
            } catch (Exception)
            {
                Console.WriteLine("[ ERROR ] : Error creating Worker node!");
            }
        }

        public void createStorageNode(string serverId, string url, int gossipDelay, int replicaId)
        {
            try
            {
                var reply = client.CreateStorageNode(
                    new CreateStorageNodeRequest
                    {
                        ServerId = serverId,
                        Url = url,
                        GossipDelay = gossipDelay,
                        ReplicaId = replicaId
                    }
                );
                if (reply.Okay)
                {
                    Console.WriteLine("[ LOG ] : Created storage node!");
                }
                else
                {
                    Console.WriteLine("[ ERROR ] : Error creating Storage node!");
                }
            }
            catch (Exception)
            {
                Console.WriteLine("[ ERROR ] : Error creating Storage node!");
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
                Console.WriteLine("[ LOG ] : Nuked Storage: " + serverId);
            }
            else
            {
                Console.WriteLine("[ ERROR ] : Error Nuking Storage: " + serverId);
            }
        }

        public void exit()
        {
            client.Nuke(new NukeAllRequest{});
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
