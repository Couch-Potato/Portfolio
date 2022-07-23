using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces.API
{
    public class ClientAPIAttribute : Attribute
    {
        public string ClientAPIName { get; }
        public ClientAPIAttribute(string name)
        {
            ClientAPIName = name;
        }
    }
}
