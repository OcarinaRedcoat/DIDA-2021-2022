using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World from WorkerNode!");
            var serverId = args[0];
            var host = args[1].Split("//")[1].Split(":")[0];
            var port = args[1].Split("//")[1].Split(":")[1];
            var gossipDelay = Int32.Parse(args[2]);
            var debug = Int32.Parse(args[3]).Equals(1);
            var logURL = args[4];

            if (debug) Console.WriteLine("Debug mode activated...");

            Console.WriteLine("Args: " + host + " : " + port);
            WorkerNodeLogic wnl = new WorkerNodeLogic(serverId, gossipDelay, debug, logURL);
            WorkerServer workerServer = new WorkerServer(serverId, host, Int32.Parse(port), ref wnl);

            Console.ReadLine();
            workerServer.ShutDown();
        }
    }
}
