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
            Console.WriteLine("========================================================================");
            Console.WriteLine("[ STARTING ] : PuppetMasterCLI!");
            Console.WriteLine("========================================================================");

            PuppetMasterLogic pm = new PuppetMasterLogic("pcs.txt");
            string line = "";

            Console.WriteLine("[ SETUP ] : Do you want to load a Config File [y/n]");
            Console.Write("[ PuppetMasterCLI ] : $ ");
            var loadFileBool = Console.ReadLine();

            // If args -> ImportFile
            if (loadFileBool == "y") {
                Console.WriteLine("[ SETUP ] : Write the name of the config file");
                Console.Write("[ PuppetMasterCLI ] : $ ");
                var configFile = Console.ReadLine();
                Console.WriteLine("[ SETUP ] : Config File: " + configFile);
                pm.ImportScriptFile(configFile);
            }

            while (run) {
                Console.WriteLine("[ PROMPT ] : Write command line");
                Console.Write("[ PuppetMasterCLI ] : $ ");
                line = Console.ReadLine();
                run = pm.ParseConfigScriptLine(line);
            }

            // Shutdown logServer
            pm.Exit();
        }

    }
}
