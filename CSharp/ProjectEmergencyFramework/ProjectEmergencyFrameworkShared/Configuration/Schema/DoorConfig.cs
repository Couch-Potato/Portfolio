using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class DoorConfig
    {
        public string Name { get; set; }
        public List<uint> Models { get; set; }
        public Location Location { get; set; }
        public float Radius { get; set; }
        public string LockedOrganization { get; set; }
        public string LockedType { get; set; }
        public bool DoAutoClose { get; set; }
        public float AutoCloseTime { get; set; }
    }
}
