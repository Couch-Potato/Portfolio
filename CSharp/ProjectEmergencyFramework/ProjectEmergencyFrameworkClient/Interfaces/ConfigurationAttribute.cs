using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces
{
    public class ConfigurationAttribute : Attribute
    {
        public string ConfigurationName;
        public ConfigurationAttribute(string n)
        {
            ConfigurationName = n;
        }
    }
}
