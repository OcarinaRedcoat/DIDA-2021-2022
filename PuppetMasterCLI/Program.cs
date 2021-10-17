using System;
using System.Collections.Generic;

namespace PuppetMasterCLI
{
    class Program
    {
        static bool run = true;

        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the PuppetMasterCLI");
            List<string> knownPCSs = new List<string>();
            knownPCSs.Add("http://localhost:10000");

            PuppetMaster pm = new PuppetMaster(knownPCSs);
            string line = "";

            Console.WriteLine("Do you want to load a Config File [y/n]");
            var loadFileBool = Console.ReadLine();

            // If args -> ImportFile
            if (loadFileBool == "y"){
                var configFile = Console.ReadLine();
                Console.WriteLine("Config File: " + configFile);
                ImportFile(pm, configFile);
            }

            while (run) {
                Console.WriteLine("Write the name of the config file");
                line = Console.ReadLine();
                ParseConfigScriptLine(pm, line);
            }

            // Shutdown logServer
            pm.Exit();

        }
        
        static void ImportFile(PuppetMaster pm, string fileName)
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

        static void ParseConfigScriptLine(PuppetMaster pm, string scritpLine)
        {
            Console.WriteLine("Script Line: " + scritpLine);
            string[] configArgs = scritpLine.Split(' ');
            Console.WriteLine("configArgs[0]: " + configArgs[0] +" configArgs[1]: " + configArgs[1] +" configArgs[2]: " + configArgs[2] +" configArgs[3]: " + configArgs[3]);
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
