using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PCS
{
    abstract class PCSLogic
    {

        public static void CreateWorkerNode(string message)
        {
            Console.WriteLine(message);
            /* TODO
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = "C:\\Users\\Vasco Faria\\source\\repos\\DIDA-2021\\Schedular\\bin\\Debug\\netcoreapp3.1\\Schedular.exe";
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
            */
        }

        public static void CreateStorageNode()
        {
            /* TODO
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = true;
            startInfo.FileName = "C:\\Users\\Vasco Faria\\source\\repos\\DIDA-2021\\Schedular\\bin\\Debug\\netcoreapp3.1\\Schedular.exe";
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
            */
        }
    }
}
