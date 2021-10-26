using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace PuppetMasterCLI
{
    public class Program
    {
        static bool run = true;

        static void Main()
        {
            Console.WriteLine("Initializing the PuppetMasterCLI");

            PuppetMasterLogic pm = new PuppetMasterLogic("pcs.txt");
            string line = "";

            Console.WriteLine("Do you want to load a Config File [y/n]");
            var loadFileBool = Console.ReadLine();

            // If args -> ImportFile
            if (loadFileBool == "y") {
                Console.WriteLine("Write the name of the config file");
                var configFile = Console.ReadLine();
                Console.WriteLine("Config File: " + configFile);
                pm.ImportScriptFile(configFile);
            }

            while (run) {
                Console.WriteLine("Write command line");
                line = Console.ReadLine();
                run = pm.ParseConfigScriptLine(line);
            }

            // Shutdown logServer
            pm.Exit();

        }

    }
}
