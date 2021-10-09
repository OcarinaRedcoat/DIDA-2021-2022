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
            startInfo.FileName = "C:\\Users\\Vasco Faria\\source\\repos\\DIDA-2021\\Scheduler\\bin\\Debug\\netcoreapp3.1\\Scheduler.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "";

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

        }
        public void CreateWorker(string serverId, string url, int gossipDelay)
        {
            // w1 http://localhost:4004

            // Ask the PCS instance with the same address as url to create a new Worker node

            // Go to the respective pcsManager

            // Ask to create a new Worker Node

            var pcsHost = url.Split("//")[1].Split(":")[0];
            Console.WriteLine(pcsHost);
            pcsManagers[pcsHost].createWorkerNode();
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
}
