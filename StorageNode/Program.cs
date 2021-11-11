using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class Program
    {
        static void Main(string[] args)
        {
            string serverId = args[0];
            string host = ExtractHostFromArg(args[1]);
            int port = ExtractPortFromArg(args[1]);
            int gossipDelay = Int32.Parse(args[2]);
            int replicaId = Int32.Parse(args[3]);

            Console.WriteLine("========================================================================");
            Console.WriteLine("[ STARTING ] : Storage Node!");
            Console.WriteLine("[ STARTING ] : ReplicaID: " + replicaId);
            Console.WriteLine("========================================================================");

            var storageLogic = new StorageNodeLogic(serverId, replicaId, gossipDelay);
            StorageServer storageServer = new StorageServer(serverId, host, port, ref storageLogic);

            Console.ReadLine();
            storageServer.ShutDown();
        }
        static string ExtractHostFromArg(string arg)
        {
            return arg.Split("//")[1].Split(":")[0];
        }

        static int ExtractPortFromArg(string arg)
        {
            return Int32.Parse(arg.Split("//")[1].Split(":")[1]);
        }
    }
}
