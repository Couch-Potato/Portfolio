using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Effects
{
    public class HealthEffectAttribute:Attribute
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public string Caption { get; set; }
        public string Icon { get; set; }
        public HealthEffectAttribute(string name, string color, string caption, string icon) {
            Name = name;
            Color = color;
            Caption = caption;
            Icon = icon;
        }
    }
}
