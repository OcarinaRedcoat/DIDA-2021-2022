using System;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World from Schedular!");

            var host = args[1].Split("//")[1].Split(":")[0];
            var port = args[1].Split("//")[1].Split(":")[1];

//            WorkerManager wm = new WorkerManager();

            SchedulerServer schedulerServer = new SchedulerServer(args[0], host, Int32.Parse(port));


//            wm.ProcessOperator();

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
