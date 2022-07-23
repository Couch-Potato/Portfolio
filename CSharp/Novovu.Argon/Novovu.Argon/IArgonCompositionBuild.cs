using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Argon
{
    public interface IArgonCompositionBuild
    {
        string Build(object handle);
    }
}
