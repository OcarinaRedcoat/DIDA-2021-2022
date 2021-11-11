using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using CHashing;
using Grpc.Core;
using System.Windows.Forms;

public delegate void DelAddMsg(string s);

namespace PuppetMasterGUI
{
    public class PuppetMasterLogic
    {
        // Init Log Server
        private LogServer logServer;

        private static List<string> knownPCSs = new List<string>();
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

        private Form1 form;

        public PuppetMasterLogic(string pcsConfigFileName, Form1 form)
        {
            this.form = form;
            logServer = new LogServer(ref this.form);
            ImportConfigFile(pcsConfigFileName);
            replicaIdCounter = 0;
            foreach (string pcsURL in knownPCSs)
            {
                var pcsHost = ExtractHostFromURL(pcsURL);
                pcsManagers.Add(pcsHost, new PCSManager(pcsURL));
            }
        }

        public void CreateScheduler(string serverId, string url)
        {
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

                foreach (PCSManager pcsManager in pcsManagers.Values)
                {
                    pcsManager.SetScheduler(url);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("[ ERROR ] : Error creating scheduler...");
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
            node.replicaId = replicaIdCounter;

            foreach (StorageNodeStruct sns in storageNodes)
            {
                try
                {
                    AddStorageRequest newStorageRequest = new AddStorageRequest { };
                    StorageInfo storageInfo = new StorageInfo
                    {
                        Id = node.serverId,
                        ReplicaId = node.replicaId,
                        Url = node.url
                    };
                    newStorageRequest.Storages.Add(storageInfo);
                    sns.storageClient.AddStorage(newStorageRequest);
                }
                catch (RpcException e)
                {
                    MessageBox.Show("[ ERROR ] - AddStorage Request Status code: " + e.StatusCode);
                    Exit();
                }
            }

            AddStorageRequest storagesRequest = new AddStorageRequest { };
            foreach (StorageNodeStruct sns in storageNodes)
            {
                StorageInfo storageInfo = new StorageInfo
                {
                    Id = sns.serverId,
                    ReplicaId = sns.replicaId,
                    Url = sns.url
                };
                storagesRequest.Storages.Add(storageInfo);
            }

            node.storageClient.AddStorage(storagesRequest);

            storageNodes.Add(node);

            replicaIdCounter++;
        }

        public void CreateWorker(string serverId, string url, int gossipDelay)
        {
            var pcsHost = ExtractHostFromURL(url);
            pcsManagers[pcsHost].createWorkerNode(serverId, url, gossipDelay, debug, logServer.GetURL());

            WorkerNodeStruct node;
            node.serverId = serverId;
            node.url = url;
            node.channel = GrpcChannel.ForAddress(url);
            node.statusClient = new StatusService.StatusServiceClient(node.channel);

            workerNodes.Add(node);

            SetupStorage.SetupStorageClient setupClient = new SetupStorage.SetupStorageClient(node.channel);

            SetupRequest setupRequest = new SetupRequest { };
            foreach (StorageNodeStruct sns in storageNodes) {
                setupRequest.Storages.Add(new StorageInfo
                {
                    Id = sns.serverId,
                    ReplicaId = sns.replicaId,
                    Url = sns.url
                });
            }
            try
            {
                SetupReply setupReply = setupClient.Setup(setupRequest);
                if (!setupReply.Okay)
                {
                    MessageBox.Show("[ ERROR ] : Error setting up the storages in worker node: " + serverId);
                }
            }
            catch (RpcException e)
            {
                MessageBox.Show("[ ERROR ] : Setup Request Status code: " + e.StatusCode);
            }
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

            List<string> storagesServerId = new List<string>();

            foreach (StorageNodeStruct sns in storageNodes)
            {
                storagesServerId.Add(sns.serverId);
            }

            ConsistentHashing consistentHasing = new ConsistentHashing(storagesServerId);


            foreach (string line in lines)
            {
                var key = line.Split(",")[0];
                var value = line.Split(",")[1];

                PopulateRequest request = new PopulateRequest();

                List<string> setOfReplicas = consistentHasing.ComputeSetOfReplicas(key);

                string firstReplica = setOfReplicas[0];

                int firstReplicaId = 0; // Checkar isto mas ira sempre existir

                foreach(StorageNodeStruct sns in storageNodes)
                {
                    if (sns.serverId == firstReplica)
                    {
                        firstReplicaId = sns.replicaId;
                    }
                }

                KeyValuePair valuePair = new KeyValuePair
                {
                    Key = key,
                    Value = value,
                    ReplicaId = firstReplicaId
                };

                request.Data.Add(valuePair);

                foreach (string replicaServerId in setOfReplicas)
                {
                    foreach(StorageNodeStruct sns in storageNodes)
                    {
                        if (replicaServerId == sns.serverId)
                        {
                            try
                            {
                                sns.storageClient.Populate(request);
                                break;
                            }
                            catch (RpcException e)
                            {
                                MessageBox.Show("[ ERROR ] : Populate Request Status code: " + e.StatusCode);
                            }
                        }
                    }
                }
            }
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
                catch (RpcException e)
                {
                    MessageBox.Show("[ ERROR ] : StatusAsync Request Status code: " + e.StatusCode);
                }
            }

            foreach (WorkerNodeStruct workerNode in workerNodes)
            {
                try
                {
                    workerNode.statusClient.StatusAsync(new StatusRequest { });
                }
                catch (RpcException e)
                {
                    MessageBox.Show("[ ERROR ] : StatusAsync Request Status code: " + e.StatusCode);
                }
            }
        }
        public void ListServer(string serverId)
        {
            // Lists all objects stored on the server identified by server id

            // Make a DumpRequest
            DumpRequest req = new DumpRequest { };
            DumpReply reply;

            foreach (StorageNodeStruct node in storageNodes)
            {
                if (node.serverId.Equals(serverId))
                {
                    try
                    {
                        reply = node.storageClient.Dump(req);
                    }
                    catch (RpcException)
                    {
                        storageNodes.Remove(node);
                        return;
                    }

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
                    string res = "Storage Server Id: " + node.serverId + "\r\n";
                    res += "key      :   versions (versionNumber, ReplicaId, valueX)\r\n";
                    foreach (KeyValuePair<string, List<DIDARecord>> pair in data)
                    {
                        res += pair.Key + " : ";
                        foreach (DIDARecord record in pair.Value)
                        {
                            res += "(" + record.Version.VersionNumber + ", " + record.Version.ReplicaId + ", " + record.Val + ")\r\n";
                        }
                    }

                    form.AddLog(res);
                }
            }
        }
        public void ListGlobal()
        {
            // Lists all objects stored on the system
            List<StorageNodeStruct> crashedNodes = new List<StorageNodeStruct>();
            string res = "key      :   versions (versionNumber, ReplicaId, valueX)\r\n";

            // For each StorageNode Client
            foreach (StorageNodeStruct node in storageNodes)
            {
                // Make a DumpRequest
                DumpRequest req = new DumpRequest { };
                DumpReply reply;
                try
                {
                    reply = node.storageClient.Dump(req);
                }
                catch (RpcException e)
                {
                    crashedNodes.Add(node);
                    continue;
                }

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
                res += "StorageId: " + node.serverId + "\r\n";
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

            // Remove crashed nodes from List
            foreach (StorageNodeStruct crashedNode in crashedNodes)
            {
                storageNodes.Remove(crashedNode);
            }
            form.AddLog(res);
        }
        public void Debug()
        {
            debug = true;
            form.debugFlag = true;
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

        /* IMPORTERS - PARSERS */

        public void ImportConfigFile(string fileName)
        {
            // Input file name. Then each line is parsed according to the specs.
            string dirFileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Scripts\\" + fileName;
            string[] lines = System.IO.File.ReadAllLines(dirFileName);

            foreach (string line in lines)
            {
                var url = ParseUrl(line);
                knownPCSs.Add(url);
            }

        }

        public void ImportScriptFile(string fileName)
        {
            // Input file name. Then each line is parsed according to the specs.
            string dirFileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\Scripts\\" + fileName;

            string[] lines = System.IO.File.ReadAllLines(dirFileName);

            foreach (string line in lines)
            {
                // Use a tab to indent each line of the file.
                ParseConfigScriptLine(line);
            }

        }

        public bool ParseConfigScriptLine(string scritpLine)
        {
            string[] configArgs = scritpLine.Split(' ');
            string command = configArgs[0];
            switch (command)
            {
                case "scheduler":
                    var schedulerId = configArgs[1];
                    var schedulerUrl = ParseUrl(configArgs[2]);
                    CreateScheduler(schedulerId, schedulerUrl);
                    break;
                case "storage":
                    var storageId = configArgs[1];
                    var storageUrl = ParseUrl(configArgs[2]);
                    var gossipDelayStorage = Int32.Parse(configArgs[3]);
                    CreateStorage(storageId, storageUrl, gossipDelayStorage);
                    break;
                case "worker":
                    var workerId = configArgs[1];
                    var workerUrl = ParseUrl(configArgs[2]);
                    var gossiDelayWorker = Int32.Parse(configArgs[3]);
                    CreateWorker(workerId, workerUrl, gossiDelayWorker);
                    break;
                case "client":
                    var inputAppFileName = configArgs[2];
                    var input = configArgs[1];
                    ClientRequest(inputAppFileName, input);
                    break;
                case "populate":
                    var dataFileName = configArgs[1];
                    Populate(dataFileName);
                    break;
                case "status":
                    Status();
                    break;
                case "listServer":
                    var serverId = configArgs[1];
                    ListServer(serverId);
                    break;
                case "listGlobal":
                    ListGlobal();
                    break;

                case "debug":
                    Debug();
                    break;
                case "crash":
                    var crashStorageId = configArgs[1];
                    Crash(crashStorageId);
                    break;
                case "wait_interval":
                    var interval = Int32.Parse(configArgs[1]);
                    Wait(interval);
                    break;
                case "exit":
                    return false;
            }
            return true;
        }

        public string ParseUrl(string url)
        {
            string host = url.Split("//")[1].Split(":")[0];
            string port = url.Split("//")[1].Split(":")[1];
            if (host.Equals("localhost"))
            {
                host = GetLocalIPAddress();
            }
            return "http://" + host + ":" + port;
        }

        public string GetLocalIPAddress()
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

    public struct StorageNodeStruct
    {
        public string serverId;
        public int replicaId;
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
