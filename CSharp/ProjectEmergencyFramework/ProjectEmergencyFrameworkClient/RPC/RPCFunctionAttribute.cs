using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.RPC
{
    public class RPCFunctionAttribute : Attribute
    {
        public string QueryName;
        public RPCFunctionAttribute(string queryName)
        {
            QueryName = queryName;
        }
    }
}
