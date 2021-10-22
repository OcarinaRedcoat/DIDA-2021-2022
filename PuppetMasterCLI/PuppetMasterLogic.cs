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
    class PuppetMasterLogic
    {
        // Init Log Server
        private LogServer logServer = new LogServer();

        private Dictionary<string, PCSManager> pcsManagers = new Dictionary<string, PCSManager>();

        // List of Workers Nodes
        private List<WorkerNodeStruct> workerNodes = new List<WorkerNodeStruct>();

        // List of Storage Nodes
        private List<StorageNodeStruct> storageNodes = new List<StorageNodeStruct>();

        private bool debug = false;
        private Process schedulerProcess;

        private SchedulerService.SchedulerServiceClient schedulerServiceClient;
        private GrpcChannel schedulerChannel;

        private int replicaIdCounter;
        public PuppetMasterLogic(List<string> pcsList)
        {
            replicaIdCounter = 0;
            foreach (string pcsURL in pcsList)
            {
                var pcsHost = ExtractHostFromURL(pcsURL);
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
            startInfo.FileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Scheduler\\bin\\Debug\\netcoreapp3.1\\Scheduler.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = serverId + " " + url;

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                schedulerProcess = Process.Start(startInfo);
                schedulerChannel = GrpcChannel.ForAddress(url);
                schedulerServiceClient = new SchedulerService.SchedulerServiceClient(schedulerChannel);
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
            var pcsHost = ExtractHostFromURL(url);
            pcsManagers[pcsHost].createStorageNode(serverId, url, gossipDelay, replicaIdCounter);

            StorageNodeStruct node;
            node.serverId = serverId;
            node.url = url;
            node.channel = GrpcChannel.ForAddress(url);
            node.storageClient = new PMStorageService.PMStorageServiceClient(node.channel);
            node.statusClient = new StatusService.StatusServiceClient(node.channel);

            storageNodes.Add(node);

            replicaIdCounter++;
        }
        public void CreateWorker(string serverId, string url, int gossipDelay)
        {
            var pcsHost = ExtractHostFromURL(url);
            Console.WriteLine("Create Worker: U: " + url + " - H: " + pcsHost);
            pcsManagers[pcsHost].createWorkerNode(serverId, url, gossipDelay, debug, logServer.GetURL());

            WorkerNodeStruct node;
            node.serverId = serverId;
            node.url = url;
            node.channel = GrpcChannel.ForAddress(url);
            node.statusClient = new StatusService.StatusServiceClient(node.channel);

            workerNodes.Add(node);
        }

        public void ClientRequest(string inputAppFileName, string input)
        {
            RunApplicationRequest request = new RunApplicationRequest
            {
                Input = input
            };

            // Parse the input AppFileName
            string dirFileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Scripts\\" + inputAppFileName;
            using (StreamReader reader = System.IO.File.OpenText(dirFileName))
            {
                string rline = String.Empty;
                while ((rline = reader.ReadLine()) != null)
                {
                    string[] line = rline.Split(" ");
                    request.Chain.Add(new DIDAOperatorID
                    {
                        Classname = line[1],
                        Order = Int32.Parse(line[2])
                    });
                }
            }
            request.ChainSize = request.Chain.Count;

            RunApplicationReply reply = schedulerServiceClient.RunApplication(request);

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

                KeyValuePair valuePair = new KeyValuePair
                {
                    Key = key,
                    Value = value
                };

                request.Data.Add(valuePair);
            }

            storageNodes[0].storageClient.Populate(request);


        }

        public string[] ParsePopulateFile(string dataFileName)
        {
            string dirFileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Scripts\\" + dataFileName;
            return File.ReadAllLines(dirFileName);
        }


        public void Status()
        {
            // Request the worker nodes to print their status
            foreach (StorageNodeStruct storageNode in storageNodes)
            {
                try
                {
                    storageNode.statusClient.StatusAsync(new StatusRequest { });
                }
                catch (Exception e)
                {
                    // TODO: Storage node down
                }
            }

            foreach (WorkerNodeStruct workerNode in workerNodes)
            {
                try
                {
                    workerNode.statusClient.StatusAsync(new StatusRequest { });
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                }
            }
        }
        public void ListServer(string serverId) // TODO: Snapshot???
        {
            // Lists all objects stored on the server identified by server id

            // Make a DumpRequest
            DumpRequest req = new DumpRequest { };
            foreach (StorageNodeStruct node in storageNodes)
            {
                if (node.serverId.Equals(serverId))
                {
                    DumpReply reply = node.storageClient.Dump(req);

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
                            if (data.TryAdd(record.Id, new List<DIDARecord>()))
                                data[record.Id].Add(record);
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
                    return;
                }
            }
        }
        public void ListGlobal()
        {
            // Lists all objects stored on the system

            // For each StorageNode Client
            foreach (StorageNodeStruct node in storageNodes)
            {
                // Make a DumpRequest
                DumpRequest req = new DumpRequest { };
                DumpReply reply = node.storageClient.Dump(req);

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
                        if (data.TryAdd(record.Id, new List<DIDARecord>()))
                            data[record.Id].Add(record);
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

            string pcsHost;
            foreach (StorageNodeStruct node in storageNodes)
            {
                if (node.serverId == serverId)
                {
                    string storageUrl = node.url;
                    pcsHost = ExtractHostFromURL(storageUrl);
                    var pcs = pcsManagers[pcsHost];
                    pcs.Crash(serverId);
                    return;
                }
            }

        }
        public void Wait(int waitInterval)
        {
            Thread.Sleep(waitInterval);
        }

        public void Exit()
        {
            foreach (KeyValuePair<string, PCSManager> pcs in pcsManagers)
            {
                pcs.Value.exit();
            }
            if (schedulerProcess != null)
                schedulerProcess.Kill();
            WaitForSchedulerProcess();
            logServer.ShutDown();
        }

        public string ExtractHostFromURL(string url)
        {
            string host = url.Split("//")[1].Split(":")[0];
            if (host.Equals("localhost"))
            {
                host = LogServer.GetLocalIPAddress();
            }
            return host;
        }

    }

    public struct StorageNodeStruct
    {
        public string serverId;
        public string url;
        public GrpcChannel channel;
        public PMStorageService.PMStorageServiceClient storageClient;
        public StatusService.StatusServiceClient statusClient;
    }

    public struct WorkerNodeStruct
    {
        public string serverId;
        public string url;
        public GrpcChannel channel;
        public StatusService.StatusServiceClient statusClient;
    }
}
