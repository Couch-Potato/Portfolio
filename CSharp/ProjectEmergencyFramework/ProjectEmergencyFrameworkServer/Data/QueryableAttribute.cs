using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkServer.Data
{
    public class QueryableAttribute : Attribute
    {
        public string QueryName;
        public QueryableAttribute(string name)
        {
            QueryName = name;
        }
    }
}
