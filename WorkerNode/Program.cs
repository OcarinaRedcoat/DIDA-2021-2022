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
            string serverId = args[0];
            string host = ExtractHostFromArg(args[1]);
            int port = ExtractPortFromArg(args[1]);
            int delay = Int32.Parse(args[2]);
            bool debug = Int32.Parse(args[3]).Equals(1);
            string logURL = args[4];

            if (debug) Console.WriteLine("Debug mode activated...");
            WorkerNodeLogic workerNodeLogic = new WorkerNodeLogic(serverId, delay, debug, logURL);
            WorkerServer workerServer = new WorkerServer(serverId, host, port, ref workerNodeLogic);

            Console.ReadLine();
            workerServer.ShutDown();
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
