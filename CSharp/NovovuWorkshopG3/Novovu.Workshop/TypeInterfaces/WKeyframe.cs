using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WKeyframe : Novovu.Controls.KeyframeEditor.KeyFrameData
    {

        public Vector3 Position = new Vector3();
        public Vector3 Rotation = new Vector3();
        public Vector3 Scale = new Vector3();
    }
}
