using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectEmergencyFrameworkClient.Interfaces
{
    /// <summary>
    /// Marks a field as reactive and will react when its UI variant is changed
    /// </summary>
    public class ReactiveAttribute : Attribute
    {
        public bool IsCustomName = false;
        public string CustomName;

        public ReactiveAttribute()
        {

        }
        public ReactiveAttribute(string custom)
        {
            CustomName = custom;
            IsCustomName = true;
        }
    }
}
