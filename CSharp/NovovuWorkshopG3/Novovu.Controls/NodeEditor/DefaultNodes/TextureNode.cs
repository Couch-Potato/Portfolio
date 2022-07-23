using Avalonia.Media;
using Avalonia.Media.Imaging;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor.DefaultNodes
{
    public class TextureNode : NodeBox
    {
        internal IBitmap Image;
        private string _source = "Assets/tex_default.jpg";
        private TextureButton TextureButton;
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (value != null)
                {
                    _source = value;
                    using (var stream = File.OpenRead(_source))
                    {
                        Image = new Bitmap(_source);
                        stream.Seek(0, SeekOrigin.Begin);
                        Outputs[0].GeneratedValue = Texture2D.FromStream(Engine.graphicsDevice, stream);
                        if (Outputs[0].Connection != null)
                        {
                            Outputs[0].Connection.Input.GeneratedValue = Outputs[0].GeneratedValue;
                        }
                    }
                    
                }
            }
        }
        public TextureNode() : base()
        {
            Title = "Texture";
            Type = "SOURCE";
            Outputs = new List<NodePoint>()
            {
                new NodePoint("Texture", "Texture", 0, this, NodePoint.PointFlowTypes.Output)
            };
            NodeYOffset = 125;
            this.ReadyFeaturePaint += Paint;
            this.RequestDrag = IntensityNode_RequestDrag;
            Size = new Avalonia.Size(150, 200);
            Image = StaticAssets.TextureDefault;

            TextureButton = new TextureButton(this);
        }

        private bool IntensityNode_RequestDrag(Avalonia.Point MousePosition, VectorCamera cam)
        {
            return TextureButton.DragBegin(MousePosition, cam.Position + Position + new Avalonia.Point(25, 40));
        }
        public void Paint(DrawingContext context, VectorCamera cam)
        {
            TextureButton.Draw(context, cam.Position + Position + new Avalonia.Point(25, 40));
        }

    }
}
