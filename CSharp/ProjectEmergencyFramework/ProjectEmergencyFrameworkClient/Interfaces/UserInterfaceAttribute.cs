using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces
{
    public class UserInterfaceAttribute : System.Attribute
    {
        public readonly string HOOK;
        public readonly bool FOCUS_REQUIRED;

        public UserInterfaceAttribute(string hook, bool focus = false)
        {
            HOOK = hook;
            FOCUS_REQUIRED = focus;
        }
    }
}
