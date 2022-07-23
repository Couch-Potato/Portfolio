using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Shared
{
    [System.AttributeUsage(System.AttributeTargets.All, AllowMultiple = false)]
    public class Property : System.Attribute
    {
        public string Name;
        public string Category;
        public string LinkedName;
        public object LinkedObject;
        public enum PropertyType { String,Enum,Number,Vector3,Bool,Execute }
        public PropertyType AttributePropertyType;

        public Property(string name, string category, PropertyType type)
        {
            Name = name;
            Category = category;
            AttributePropertyType = type;
        }
        public object Get()
        {
            Type AT = LinkedObject.GetType();

            return AT.GetProperty(LinkedName).GetValue(LinkedObject);
        }
        public void Set(object value)
        {
            Type AT = LinkedObject.GetType();
            AT.GetProperty(LinkedName).SetValue(LinkedObject, value);
        }
        public static Property BuildPropertyFromData(CustomAttributeData data, object Linked, string lname)
        {
            Property p = new Property((string)data.ConstructorArguments[0].Value, (string)data.ConstructorArguments[1].Value, (PropertyType)data.ConstructorArguments[2].Value);
            p.LinkedObject = Linked;
            p.LinkedName = lname;
            return p;
        }

    }
}
