using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces.Serialized
{
    [Serializable]
    public class SScript
    {
        public string Name;
        public string Content;
        public static SScript From(WScript script)
        {
            return new SScript()
            {
                Name = script.Name,
                Content = script.Source
            };
        }
        public WScript Import(object parent)
        {
            //WScript s = new WScript(Content, parent, Name);

            return new WScript();
        }
    }
}
