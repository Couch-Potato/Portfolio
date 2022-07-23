using Avalonia.Media.Imaging;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Xenon.ScriptEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public class ComponentAsset : AssetItem
    {
        public IComponent LinkedComponent;
        public ComponentAsset(IControlAssetTab parent, IComponent linked) : base(parent)
        {
            Attachment = linked;
            LinkedComponent = linked;
            Name = linked.Name;
            Icon = new Bitmap("Assets/COMPONENT.png");
        }
    }
}
