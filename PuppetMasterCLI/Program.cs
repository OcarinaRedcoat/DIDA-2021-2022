using System;

namespace PuppetMasterCLI
{
    class Program
    {
        static bool run = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the PuppetMasterCLI");
            PuppetMaster pm = new PuppetMaster();
            string line = "";

            // If args -> ImportFile

            // Init Log Server
            LogServer ls = new LogServer();

            while (run) {
                line = Console.ReadLine();
                ParseConfigScriptLine(pm, line);
            }

            // Shutdown logServer
            ls.ShutDown();

        }
        
        static void ImportFile(PuppetMaster pm, string fileName)
        {
            // Input file name. Then each line is parsed according to the specs.
            string[] lines = System.IO.File.ReadAllLines(fileName);
        
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                ParseConfigScriptLine(pm, line);
            }

        }

        static void ParseConfigScriptLine(PuppetMaster pm, string scritpLine)
        {
            string[] configArgs = scritpLine.Split(' ');
            string command = configArgs[0];
            switch (command)
            {
                case "scheduler":
                    pm.CreateScheduler(configArgs[1], configArgs[2]);
                    break;
                case "storage":
                    pm.CreateStorage(configArgs[1], configArgs[2], Int32.Parse(configArgs[3]));
                    break;
                case "worker":
                    pm.CreateWorker(configArgs[1], configArgs[2], Int32.Parse(configArgs[3]));
                    break;
                case "client":
                    pm.ClientRequest(configArgs[1]);
                    break;
                case "populate":
                    pm.Populate(configArgs[1]);
                    break;
                case "status":
                    pm.Status();
                    break;
                case "listServer":
                    pm.ListServer(configArgs[1]);
                    break;
                case "listGlobal":
                    pm.ListGlobal();
                    break;

                case "debug":
                    pm.Debug();
                    break;
                case "crash":
                    pm.Crash(configArgs[1]);
                    break;
                case "wait_interval":
                    pm.Wait(Int32.Parse(configArgs[1]));
                    break;
                case "exit":
                    run = false;
                    break;
            }
        }
    }
}
