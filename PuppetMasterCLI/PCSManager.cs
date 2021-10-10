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
        private PCSService.PCSServiceClient client;

        public PCSManager(string url)
        {
            this.url = url;
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress(url);
            client = new PCSService.PCSServiceClient(channel);
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
    }
}
