using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Missions
{
    public class MissionAttribute : Attribute
    {
        public string Name { get; set; }
        public bool AllowOnlyMission { get; set; }
        public MissionAttribute(string name, bool requireOnlyOne)
        {
            Name = name;
            AllowOnlyMission = requireOnlyOne;
        }
    }
}
