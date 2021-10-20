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

            SchedulerServer schedulerServer = new SchedulerServer(args[0], host, Int32.Parse(port));

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
