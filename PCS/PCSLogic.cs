using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PCS
{
    abstract class PCSLogic
    {
        private static ConcurrentDictionary<string, Process> processes  = new ConcurrentDictionary<string, Process>();

        public static void CreateWorkerNode(string serverId, string url, int gossipDelay)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\WorkerNode\\bin\\Debug\\netcoreapp3.1\\WorkerNode.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = serverId + " " + url + " " + gossipDelay;

            try
            {
                processes.TryAdd(serverId, Process.Start(startInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void CreateStorageNode(string serverId, string url, int gossipDelay)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = Directory.GetCurrentDirectory() + "\\..\\..\\..\\..\\StorageNode\\bin\\Debug\\netcoreapp3.1\\StorageNode.exe";
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = serverId + " " + url + " " + gossipDelay;

            try
            {
                processes.TryAdd(serverId, Process.Start(startInfo));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public static void NukeStorage(string serverId)
        {
            Process storageProcess = processes[serverId];
            storageProcess.Kill();
        }

        public static void Nuke()
        {
            foreach (KeyValuePair<string, Process> proc in processes)
            {
                proc.Value.Kill();   
            }
        }


        public static void WaitForProcesses() { lock (processes) { foreach (KeyValuePair<string, Process> k in processes) { k.Value.WaitForExit(); } } }
    }
}
