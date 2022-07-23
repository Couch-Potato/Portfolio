using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public interface IBoundHolder
    {
        BoundingBox GetBoundingBox();
    }
}
