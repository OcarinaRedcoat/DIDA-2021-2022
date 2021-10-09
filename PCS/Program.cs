using System;

namespace PCS
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new PCSServer();
            Console.WriteLine("Hello World from PCS!");
            Console.ReadLine();
            server.ShutDown();
        }
    }
}
