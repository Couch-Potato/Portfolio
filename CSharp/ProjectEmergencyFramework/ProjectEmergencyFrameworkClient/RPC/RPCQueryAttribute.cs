using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.RPC
{
    public class RPCQueryAttribute : Attribute
    {
        public string QueryName { get; set; }
        public RPCQueryAttribute(string name)
        {
            QueryName = name;
        }
    }
}
