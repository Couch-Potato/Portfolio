using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Logging
{
    public class Logger
    {
        public delegate void LogDelegate(LogTypes type, string time, string from, string message);

        public static LogDelegate LogHandler;

        public enum LogTypes { Error, Message, Warning, Script}

        public static void Log(LogTypes log, object from, string message)
        {
            string time = DateTime.Now.ToString("h:mm:ss tt");
            if (LogHandler != null)
            {
                LogHandler(log, time, from.GetType().FullName, message);
            }
            
        }
    }
}
