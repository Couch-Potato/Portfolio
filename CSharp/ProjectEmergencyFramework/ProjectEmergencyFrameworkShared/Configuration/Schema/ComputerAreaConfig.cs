using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class ComputerAreaConfig
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public float Radius { get; set; }
        public string ComputerType { get; set; }
        public List<uint> ModelHashes { get; set; }
    }
}
