using System;
using System.Collections.Generic;
using System.Text;

namespace StorageNode
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World from Storage Node!");
            var host = args[1].Split("//")[1].Split(":")[0];
            var port = args[1].Split("//")[1].Split(":")[1];
            StorageServer storageServer = new StorageServer(args[0], host, Int32.Parse(port));
            Console.ReadLine();
            storageServer.ShutDown();
        }
    }
}
