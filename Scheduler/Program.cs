using System;

namespace Scheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World from Schedular!");

            WorkerManager wm = new WorkerManager("http://localhost:3001");
            Console.WriteLine("Calling: wm.ProcessOperator");
            
            wm.ProcessOperator();

            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
