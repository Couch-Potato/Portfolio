using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Materials
{
    public interface IMaterial
    {
        Effect GetMaterialEffect(Matrix World, Matrix View, Matrix Projection,Matrix transpose);
    }
}
