using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PCS
{
    class PCSLogic
    {
        private ConcurrentDictionary<string, Process> processes  = new ConcurrentDictionary<string, Process>();

        public CreateWorkerNodeReply CreateWorkerNode(string serverId, string url, int delay, bool debug, string logURL)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\WorkerNode\\bin\\Debug\\netcoreapp3.1\\WorkerNode.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = serverId + " " + url + " " + delay + " " + (debug ? "1" : "0") + " " + logURL;

            try
            {
                Console.WriteLine("[ LOG ] - Creating Worker...");
                processes.TryAdd(serverId, Process.Start(startInfo));
            }
            catch (Exception)
            {
                Console.WriteLine("[ ERROR ] : CreateWorkerNode Process failed.");
            }
            return new CreateWorkerNodeReply
            {
                Okay = true
            };
        }

        public CreateStorageNodeReply CreateStorageNode(string serverId, string url, int gossipDelay, int replicaId)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\StorageNode\\bin\\Debug\\netcoreapp3.1\\StorageNode.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = serverId + " " + url + " " + gossipDelay + " " + replicaId;

            try
            {
                Console.WriteLine("[ LOG ] - Creating Storage...");
                processes.TryAdd(serverId, Process.Start(startInfo));
            }
            catch (Exception)
            {
                Console.WriteLine("[ ERROR ] : CreateStorageNode Process failed.");
            }
            return new CreateStorageNodeReply
            {
                Okay = true
            };
        }

        public NukeReply NukeStorage(string serverId)
        {
            Process storageProcess = processes[serverId];
            storageProcess.Kill();
            return new NukeReply
            {
                Okay = true
            };
        }

        public NukeAllReply Nuke()
        {
            foreach (KeyValuePair<string, Process> proc in processes)
            {
                proc.Value.Kill();   
            }
            return new NukeAllReply { };
        }

        public void WaitForProcesses() { lock (processes) { foreach (KeyValuePair<string, Process> k in processes) { k.Value.WaitForExit(); } } }
    }
}
