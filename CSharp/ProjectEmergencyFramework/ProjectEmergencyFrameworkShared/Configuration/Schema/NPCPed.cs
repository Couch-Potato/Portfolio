using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class NPCPed
    {
        public string Name { get; set; }
        public Location Location { get; set; }
        public string MissionTrigger { get; set; }
        public string PedModel { get; set; }
        public bool IsSpawnedOnStart { get; set; }
        public float Heading { get; set; }
    }
}
