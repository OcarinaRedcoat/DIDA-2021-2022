using System;
using System.Net;
using System.Net.Sockets;

namespace PCS
{
    class Program
    {
        static void Main(string[] args)
        {
            PCSLogic pcsLogic = new PCSLogic();
            var server = new PCSServer(ref pcsLogic);
            Console.WriteLine("========================================================================");
            Console.WriteLine("[ STARTING ] : Process Creation Service (" + GetLocalIPAddress()  + ") !");
            Console.WriteLine("========================================================================");
            Console.ReadLine();
            server.ShutDown();
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
