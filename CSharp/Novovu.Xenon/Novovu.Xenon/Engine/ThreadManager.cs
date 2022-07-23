using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class ThreadManager
    {
        public static Dictionary<string, XenonThread> RepeatThreads = new Dictionary<string, XenonThread>();
    }
    public class XenonThread
    {
        public delegate void XenonThreadDelegate();
        public List<XenonThreadDelegate> ThreadRunTasks = new List<XenonThreadDelegate>();
        public List<XenonThreadDelegate> ThreadRunItems = new List<XenonThreadDelegate>();
        public void Run()
        {
            foreach (XenonThreadDelegate td in ThreadRunTasks)
            {
                td();
            }
            foreach (XenonThreadDelegate td in ThreadRunItems)
            {
                td();
            }
            ThreadRunItems.Clear();
        }
    }
}
