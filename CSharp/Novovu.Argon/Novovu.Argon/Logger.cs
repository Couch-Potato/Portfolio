using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public class Logger
    {
        public delegate void LogEventDelegate(string caller, string time,string message, string msgtype);

        public static event LogEventDelegate OnLog;

        public static void Log(object caller, string message, string msgtype="MESSAGE")
        {
            if (OnLog != null)
            {
                string time = DateTime.Now.ToString("h:mm:ss tt");
                OnLog.Invoke(caller.GetType().FullName, time, message, msgtype);
            }
        }
    }
}
