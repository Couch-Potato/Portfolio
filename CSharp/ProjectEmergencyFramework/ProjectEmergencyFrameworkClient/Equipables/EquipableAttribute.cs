using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Equipables
{
    public class EquipableAttribute : Attribute
    {
        public string Name { get; }
        public string Icon { get; }
        public bool Stackable { get; }
        public EquipableAttribute(string name, string icon, bool stackable = false)
        {
            Name = name;
            Icon = icon;
            Stackable = stackable;
        }
    }
}
