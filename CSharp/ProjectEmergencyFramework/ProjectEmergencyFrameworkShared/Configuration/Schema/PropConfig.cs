using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class PropConfig
    {
        public string Name { get; set; }
        public string PropFile { get; set; }
        public bool CreateStaticPropOnStart { get; set; }
    }
    public class HydratedPropConfig
    {
        public string Name { get; set; }
        public bool CreateStaticPropOnStart { get; set; }
        public List<Placement> Placements { get; set; }
    }
}
