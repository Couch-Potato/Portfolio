using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public class ArgonComposition
    {
        public string Source;
        public string Name;
        public ArgonComposition(string name, byte[] src)
        {
            Source = Encoding.UTF8.GetString(src);
            Name = name;
        }
    }
}
