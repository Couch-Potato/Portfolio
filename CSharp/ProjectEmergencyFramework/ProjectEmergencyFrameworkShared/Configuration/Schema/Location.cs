using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProjectEmergencyFrameworkShared.Configuration.Schema
{
    public class Location
    {
        [XmlAttribute]
        public float X { get; set; } = 0;
        [XmlAttribute]
        public float Y { get; set; } = 0;
        [XmlAttribute]
        public float Z { get; set; } = 0;
    }
    public class Color
    {
        [XmlAttribute]
        public float R { get; set; } = 0;
        [XmlAttribute]
        public float G { get; set; } = 0;
        [XmlAttribute]
        public float B { get; set; } = 0;
    }
}
