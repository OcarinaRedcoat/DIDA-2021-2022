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
            var host = args[1].Split("//")[1].Split(":")[0];
            var port = args[1].Split("//")[1].Split(":")[1];

            Console.WriteLine("Args: " + host + " : " + port);
            WorkerServer workerServer = new WorkerServer(args[0], host, Int32.Parse(port));

            Console.ReadLine();
            workerServer.ShutDown();
        }
    }
}
