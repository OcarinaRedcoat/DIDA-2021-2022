using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PuppetMasterCLI
{
    class PuppetMaster
    {
        // Init Log Server
        private LogServer ls = new LogServer();

        private Dictionary<string, PCSManager> pcsManagers = new Dictionary<string, PCSManager>();

        // List of Workers Nodes  
        private List<string> workerNodes = new List<string>();

        // List of Storage Nodes
        List<StorageNodeStruct> storageNodes = new List<StorageNodeStruct>();

        private bool debug = false;
        private Process schedulerProcess;

        public PuppetMaster(List<string> pcsList)
        {
            foreach (string pcsURL in pcsList)
            {
                var pcsHost = pcsURL.Split("//")[1].Split(":")[0];
                pcsManagers.Add(pcsHost, new PCSManager(pcsURL));
            }
        }

        public void CreateScheduler(string serverId, string url)
        {

            Console.WriteLine("Create Schedular: ", serverId, url);
            // Init the Schedular in a new process
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = Directory.GetCurrentDirectory() +  "\\..\\..\\..\\..\\Scheduler\\bin\\Debug\\netcoreapp3.1\\Scheduler.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = serverId + " " + url;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                schedulerProcess = Process.Start(startInfo);
            }
            catch (Exception e)
            {
                // Log error.
                Console.WriteLine(e.Message);
            }
        }

        private void WaitForSchedulerProcess()
        {
            if (schedulerProcess != null)
            {
                schedulerProcess.WaitForExit();
            }
        }

        public void CreateStorage(string serverId, string url, int gossipDelay)
        {
            var pcsHost = url.Split("//")[1].Split(":")[0];
            pcsManagers[pcsHost].createStorageNode(serverId, url, gossipDelay);

            StorageNodeStruct node;
            node.serverId = serverId;
            node.url = url;
            storageNodes.Add(node);

        }
        public void CreateWorker(string serverId, string url, int gossipDelay)
        {
            var pcsHost = url.Split("//")[1].Split(":")[0];
            pcsManagers[pcsHost].createWorkerNode(serverId, url, gossipDelay);
        }
        public void ClientRequest(string inputAppFileName)
        {
            // Fake a client request
            // Parse the input AppFileName or let the Schedular do that ???
            // Ask the schedular to manage the request
        }
        public void Populate(string dataFileName)
        {
            // Parse the file
            // Go to all pcsManagers
            // Populate all the Storage Nodes with the data in the file

            var lines = ParsePopulateFile(dataFileName);

            PopulateRequest request = new PopulateRequest();

            foreach (string line in lines)
            {
                var key = line.Split(",")[0];
                var value = line.Split(",")[1];
                Console.WriteLine("Line: " + line);
                Console.WriteLine("Key: " + key + " Value: " + value);


                KeyValuePair valuePair = new KeyValuePair
                {
                    Key = key,
                    Value = value
                };

                request.Data.Add(valuePair);
            }
            
            var url = storageNodes[0].url;

            GrpcChannel channel = GrpcChannel.ForAddress(url);
            var client = new PMStorageService.PMStorageServiceClient(channel);

            client.Populate(request);


        }

        public string[] ParsePopulateFile(string dataFileName)
        {
            string[] lines = System.IO.File.ReadAllLines(dataFileName);
            return lines;
        }


        public void Status() {
            // Request the worker nodes to print their status
        }
        public void ListServer(string serverId)
        {
            // Lists all objects stored on the server identified by server id
        }
        public void ListGlobal()
        {
            // Lists all objects stored on the system
        }
        public void Debug()
        {
            debug = true;
        }
        public void Crash(string serverId)
        {
            // Force a storage process to terminate
        }
        public void Wait(int waitInterval)
        {
            Thread.Sleep(waitInterval);
        }

        public void Exit()
        {
            WaitForSchedulerProcess();
            ls.ShutDown();
        }
    }

    public struct StorageNodeStruct
    {
        public string serverId;
        public string url;
    }
}
