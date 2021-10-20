using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private List<string> storageNodes = new List<string>();

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
            // https://stackoverflow.com/questions/9679375/how-can-i-run-an-exe-file-from-my-c-sharp-code
            Console.WriteLine("Create Schedular: ", serverId, url);
            // Init the Schedular in a new process
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = "C:\\Users\\Caetano\\source\\repos\\DIDA-2021\\Scheduler\\bin\\Debug\\netcoreapp3.1\\Scheduler.exe";
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
            // Ask the PCS instance with the same address as url to create a new Storage node

            // Go to the respective pcsManager

            // Ask to create a new Storage Node
            var pcsHost = url.Split("//")[1].Split(":")[0];
            pcsManagers[pcsHost].createStorageNode(serverId, url, gossipDelay);

        }
        public void CreateWorker(string serverId, string url, int gossipDelay)
        {
            // w1 http://localhost:4004

            // Ask the PCS instance with the same address as url to create a new Worker node

            // Go to the respective pcsManager

            // Ask to create a new Worker Node

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
        }
        public void Status() {
            // Request the worker nodes to print their status
        }
        public void ListServer(string serverId)
        {
            // Lists all objects stored on the server identified by server id

            // Get the client from StorageNode Clients List by its serverId
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:3000");
            var client = new DatabaseService.DatabaseServiceClient(channel);

            // Make a DumpRequest
            DumpRequest req = new DumpRequest { };
            DumpReply reply = client.Dump(req);
            
            Dictionary<string, List<DIDARecord>> data = new Dictionary<string, List<DIDARecord>>();
            foreach (DIDARecord record in reply.Data)
            {
                if (data.ContainsKey(record.Id))
                {
                    // Check if the list already has the version
                    data[record.Id].Add(record);
                }
                else
                {
                    data.TryAdd(record.Id, new List<DIDARecord>());
                }
            }

            // Print the results like:
            // key    |   versions (versionNumber, ReplicaId, valueX)
            // money  |   (1, 1, 1000) (2, 1, 2000)
            String res = "key      :   versions (versionNumber, ReplicaId, valueX)";
            foreach (KeyValuePair<string, List<DIDARecord>> pair in data)
            {
                res += pair.Key + " : ";
                foreach (DIDARecord record in pair.Value)
                {
                    res += "(" + record.Version.VersionNumber + ", " + record.Version.ReplicaId + ", " + record.Val + ")\r\n";
                }
            }

            Console.WriteLine(res);
        }
        public void ListGlobal()
        {
            // Lists all objects stored on the system

            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            GrpcChannel channel = GrpcChannel.ForAddress("http://localhost:3000");
            var mockClient = new DatabaseService.DatabaseServiceClient(channel);

            List<DatabaseService.DatabaseServiceClient> clients = new List<DatabaseService.DatabaseServiceClient>();
            clients.Add(mockClient);

            // For each StorageNode Client
            foreach (DatabaseService.DatabaseServiceClient client in clients)
            {
                // Make a DumpRequest
                DumpRequest req = new DumpRequest { };
                DumpReply reply = client.Dump(req);

                Dictionary<string, List<DIDARecord>> data = new Dictionary<string, List<DIDARecord>>();
                foreach (DIDARecord record in reply.Data)
                {
                    if (data.ContainsKey(record.Id))
                    {
                        // Check if the list already has the version
                        data[record.Id].Add(record);
                    }
                    else
                    {
                        data.TryAdd(record.Id, new List<DIDARecord>());
                    }
                }

                // Print the results like:
                // key    |   versions (versionNumber, ReplicaId, valueX)
                // money  |   (1, 1, 1000) (2, 1, 2000)
                String res = "key      :   versions (versionNumber, ReplicaId, valueX)";
                foreach (KeyValuePair<string, List<DIDARecord>> pair in data)
                {
                    res += pair.Key + " : ";
                    foreach (DIDARecord record in pair.Value)
                    {
                        res += "(" + record.Version.VersionNumber + ", " + record.Version.ReplicaId + ", " + record.Val + ")\r\n";
                    }
                }

                Console.WriteLine(res);
            }
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
}
