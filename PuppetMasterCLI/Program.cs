using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace PuppetMasterCLI
{
    class Program
    {
        static bool run = true;
        private static List<string> knownPCSs = new List<string>();

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the PuppetMasterCLI");
            // Config file to each PCS, NOT GET LOCAL IP
            ImportConfigFile("pcs.txt");


            PuppetMasterLogic pm = new PuppetMasterLogic(knownPCSs);
            string line = "";

            Console.WriteLine("Do you want to load a Config File [y/n]");
            var loadFileBool = Console.ReadLine();

            // If args -> ImportFile
            if (loadFileBool == "y"){
                Console.WriteLine("Write the name of the config file");
                var configFile = Console.ReadLine();
                Console.WriteLine("Config File: " + configFile);
                ImportFile(pm, configFile);
            }

            while (run) {
                Console.WriteLine("Write command line");
                line = Console.ReadLine();
                ParseConfigScriptLine(pm, line);
            }

            // Shutdown logServer
            pm.Exit();

        }

        static void ImportConfigFile(string fileName)
        {
            // Input file name. Then each line is parsed according to the specs.
            string[] lines = System.IO.File.ReadAllLines(fileName);

            foreach (string line in lines)
            {
                var url = ParseUrl(line);
                knownPCSs.Add(url);
            }

        }

        static void ImportFile(PuppetMasterLogic pm, string fileName)
        {
            // Input file name. Then each line is parsed according to the specs.
            string[] lines = System.IO.File.ReadAllLines(fileName);
        
            foreach (string line in lines)
            {
                Console.WriteLine("Line: " + line);
                // Use a tab to indent each line of the file.
                ParseConfigScriptLine(pm, line);
            }

        }

        static void ParseConfigScriptLine(PuppetMasterLogic pm, string scritpLine)
        {
            Console.WriteLine("Script Line: " + scritpLine);
            string[] configArgs = scritpLine.Split(' ');
            string command = configArgs[0];
            switch (command)
            {
                case "scheduler":
                    var schedulerId = configArgs[1];
                    var schedulerUrl = ParseUrl(configArgs[2]);
                    pm.CreateScheduler(schedulerId, schedulerUrl);
                    break;
                case "storage":
                    var storageId = configArgs[1];
                    var storageUrl = ParseUrl(configArgs[2]);
                    var gossipDelayStorage = Int32.Parse(configArgs[3]);
                    pm.CreateStorage(storageId, storageUrl, gossipDelayStorage);
                    break;
                case "worker":
                    var workerId = configArgs[1];
                    var workerUrl = ParseUrl(configArgs[2]);
                    var gossiDelayWorker = Int32.Parse(configArgs[3]);
                    pm.CreateWorker(workerId, workerUrl, gossiDelayWorker);
                    break;
                case "client":
                    var inputAppFileName = configArgs[2];
                    var input = configArgs[1];
                    pm.ClientRequest(inputAppFileName, input);
                    break;
                case "populate":
                    var dataFileName = configArgs[1];
                    pm.Populate(dataFileName);
                    break;
                case "status":
                    pm.Status();
                    break;
                case "listServer":
                    var serverId = configArgs[1];
                    pm.ListServer(serverId);
                    break;
                case "listGlobal":
                    pm.ListGlobal();
                    break;

                case "debug":
                    pm.Debug();
                    break;
                case "crash":
                    var crashStorageId = configArgs[1];
                    pm.Crash(crashStorageId);
                    break;
                case "wait_interval":
                    var interval = Int32.Parse(configArgs[1]);
                    pm.Wait(interval);
                    break;
                case "exit":
                    run = false;
                    break;
            }
        }

        public static string ParseUrl(string url)
        {
            string host = url.Split("//")[1].Split(":")[0];
            string port = url.Split("//")[1].Split(":")[1];
            if (host.Equals("localhost"))
            {
                host = GetLocalIPAddress();
            }
            return "http://" + host + ":" + port;
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
