using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class Holster
    {
        public string WeaponName { get; set; }
        public HolsterComponent WeaponInHolster { get; set; }
        public HolsterComponent WeaponOutOfHolster { get; set; }
    }
    public class HolsterComponent
    {
        [XmlAttribute]
        public int ComponentId { get; set; }
        [XmlAttribute]
        public int DrawableId { get; set; }
    }
}
