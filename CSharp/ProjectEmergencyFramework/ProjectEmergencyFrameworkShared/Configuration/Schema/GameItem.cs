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
    
    public class GameItem
    {
        public string Name { get; set; }
        public string SmartItemId { get; set; }
        public string IconId { get; set; }
        public GlobalPropertyList Properties { get; set; }
    }
}
