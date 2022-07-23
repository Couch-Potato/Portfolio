using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class PropPlacementConfig
    {
        public string PlacementName { get; set; }
        public string PropName { get; set; }
        public WorldPlacement WorldPlacement { get; set; }
        
    }
    public class WorldPlacement
    {
        public Location Location { get; set; }
        public Location Orientation { get; set; }
    }
}
