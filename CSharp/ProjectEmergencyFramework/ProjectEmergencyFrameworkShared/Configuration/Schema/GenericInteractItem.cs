using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class GenericInteractItem
    {
        public string Name { get; set; }
        public string InteractId { get; set; }
        public Location Location { get; set; }
        public GlobalPropertyList GlobalPropertyList { get; set; }
    }
}
