using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public interface IManipulatable
    {
        void SetPosition(Vector3 Position);
        Vector3 GetPosition();

        void SetScale(Vector3 Scale);
        Vector3 GetScale();
    }
}
