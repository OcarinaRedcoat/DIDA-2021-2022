using System;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World from Schedular!");

            var schedulerId = args[0];
            var host = ExtractHostFromArg(args[1]);
            var port = ExtractPortFromArg(args[1]);

            SchedulerServer schedulerServer = new SchedulerServer(schedulerId, host, port);

            Console.ReadLine();
        }

        static string ExtractHostFromArg(string arg)
        {
            return arg.Split("//")[1].Split(":")[0];
        }

        static int ExtractPortFromArg(string arg)
        {
            return Int32.Parse(arg.Split("//")[1].Split(":")[1]);
        }
    }
}
