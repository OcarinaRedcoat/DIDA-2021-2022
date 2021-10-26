using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PuppetMasterGUI
{
    class LogService : LogServerService.LogServerServiceBase
    {
        private Form1 form;
        public LogService(ref Form1 form)
        {
            this.form = form;
        }

        public LogReply log(string workerId, string message)
        {
            Console.WriteLine("LOG: WorkerId(" + workerId + ") : " + message);
            this.form.BeginInvoke(new DelAddMsg(form.AddLog), new object[] {
                "LOG: WorkerId(" + workerId + ") : " + message }
            );
            return new LogReply
            {
                Okay = true
            };
        }

        public override Task<LogReply> Log(LogRequest request, ServerCallContext context)
        {
            return Task.FromResult(log(request.WorkerId, request.Message));
        }
    }
    class LogServer
    {
        private Int32 port = 10001;
        private string host = GetLocalIPAddress(); // TODO: Care with other machin

        Server server;
        Form1 form;
        
        public LogServer(ref Form1 form)
        {
            this.form = form;
            server = new Server
            {
                Services = { LogServerService.BindService(new LogService(ref this.form)) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };
            server.Start();
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

        public string GetURL()
        {
            return "http://" + host + ":" + port;
        }

        public void ShutDown()
        {
            server.ShutdownAsync().Wait();
        }
    }
}
