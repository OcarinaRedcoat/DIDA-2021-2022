using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PuppetMasterCLI
{
    class PuppetMaster
    {
        private List<string> workerNodes  = new List<string>();
        private List<string> storageNodes = new List<string>();
        private List<string> pcsInstances = new List<string>();

        private bool debug = false;

        public void CreateScheduler(string serverId, string url)
        {
            // Init the Schedular in a new process
        }
        public void CreateStorage(string serverId, string url, int gossipDelay)
        {
            // Ask the PCS instance with the same address as url to create a new Storage node
        }
        public void CreateWorker(string serverId, string url, int gossipDelay)
        {
            // Ask the PCS instance with the same address as url to create a new Worker node
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
    }
}
