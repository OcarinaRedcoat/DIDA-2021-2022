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
            WorkerServer workerServer = new WorkerServer(args[0], host, Int32.Parse(port));

            // Test Storage
            StorageManager sm = new StorageManager("http://localhost:3000");
            sm.WriteToStorage("key", "value of key");

            var reply = sm.ReadFromStorage("key", null); // Doesnt work for null...

            Console.WriteLine(reply.val);

            Console.ReadLine();
            workerServer.ShutDown();
        }
    }
}
