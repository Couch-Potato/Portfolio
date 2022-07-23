using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class GlobalPropertyList
    {
        public string desc { get; set; }
        public List<string> tags { get; set; }
        public string icon { get; set; }
        public string type { get; set; }
        public int amount { get; set; }
        public string name { get; set; }
        public string weapon_hash { get; set; }
        public string door { get; set; }
        public string organization { get; set; }
        public bool organizationLocked { get; set; }
        public bool onlyShowOffDuty { get; set; }
        public bool requireOnTeam { get; set; }
    }
}
