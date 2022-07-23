using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peridot
{
    public class PDLogger
    {
        public static int LogLevel = 3;
        public static void Log(string log, int logLevel, string stack="")
        {
            if (logLevel <= LogLevel)
            {
                switch (logLevel)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {log}");
                        if (LogLevel == 5)
                        {
                            Console.WriteLine(stack);
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                        Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [ERROR] {log}");
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [WARNING] {log}");
                        Console.ForegroundColor = ConsoleColor.White;
                        Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [WARNING] {log}");
                        break;
                    case 3:
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [NOTICE] {log}");
                        Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [NOTICE] {log}");
                        break;
                    case 4:
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [DBG] {log}");
                        Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [DBG] {log}");
                        break;
                    case 5:
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} [STACK] {log}");
                        Debug.WriteLine($"{DateTime.Now.ToLongTimeString()} [STACK] {log}");
                        break;
                }
            }
        }
    }
}
