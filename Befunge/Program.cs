using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Befunge
{
    class Program
    {
        static void Main(string[] args)
        {
            ProgramState s = new ProgramState("insult.bf");
            s.Run();
            Console.ReadKey();
        }
    }
}
