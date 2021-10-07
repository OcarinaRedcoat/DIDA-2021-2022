using System;

namespace PuppetMasterCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing the PuppetMasterCLI");
      
            while (true) { 
               
            }
        }
        
        static void ImportFile(string fileName)
        {
            // Input file name. Then each line is parsed according to the specs.
            string[] lines = System.IO.File.ReadAllLines(fileName);

            // Display the file contents by using a foreach loop.
        
            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                ParseConfigScriptLine(line);
            }

        }

        static void ParseConfigScriptLine(string scritpLine)
        {
            stri
            string[] configArgs = text.Split(' ');
            string command = configArgs[0]
            switch (command)
            {
                case "scheduler":
                    CreateScheduler(configArgs[1], configArgs[2]);
                    break;
                case "storage":
                    CreateStorage(configArgs[1], configArgs[2], Int32.Parse(configArgs[3]));
                    break;
                case "worker":
                    CreateWorker(configArgs[1], configArgs[2], Int32.Parse(configArgs[3]));
                    break;
                case "client":
                    ClientRequest(configArgs[1]);
                    break;
                case "populate":
                    Populate(configArgs[1]);
                    break;
                case "status":
                    Status();
                    break;
                case "listServer":
                    ListServer(configArgs[1]);
                    break;
                case "listGlobal":
                    ListGlobal();
                    break;

                case "debug":
                    Debug();
                    break;
                case "crash":
                    Crash(configArgs[1]);
                    break;
                case "wait_interval":
                    Wait(Int32.Parse(configArgs[1]));
                    break;
            }
        }

        static void CreateScheduler(string serverId, string url) { }
        static void CreateStorage(string serverId, string url, int gossipDelay) { }
        static void CreateWorker(string serverId, string url, int gossipDelay) { }
        static void ClientRequest(string inputAppFileName) { }
        static void Populate(string dataFileName) { }
        static void Status() { }
        static void ListServer(string serverId) { }
        static void ListGlobal() { }
        static void Debug() { }
        static void Crash(string serverId) { }
        static void Wait(int waitInterval) { }
    }
}
