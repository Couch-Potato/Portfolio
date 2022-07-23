using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor.DefaultNodes
{
    internal class TextureButton
    {
        private TextureNode Parent;
        public TextureButton(TextureNode n)
        {
            Parent = n;
        }
        public void Draw(DrawingContext context, Point Positon)
        {
            context.DrawImage(Parent.Image, new Rect(new Point(0,0), Parent.Image.Size), new Rect(Positon, new Size(100,100)), Avalonia.Visuals.Media.Imaging.BitmapInterpolationMode.LowQuality);
        }
        public bool DragBegin(Point MousePosition, Point Positon)
        {
            if (new Rect(Positon, new Size(100,100)).Contains(MousePosition))
            {
                SetImage();
                return true;
            }
            return false;
        }
        public async void SetImage()
        {
            var t = await StaticAssets.ResolveImageFile();
            if (t != null)
            {
                Parent.Source = t;
            }
            
        }
    }
}
