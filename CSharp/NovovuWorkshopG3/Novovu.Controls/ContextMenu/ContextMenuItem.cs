using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.ContextMenu
{
    public class ContextMenuItem
    {
        public Brush brush = new SolidColorBrush(new Color(255, 33, 35, 55));
        public string Name = "Pencil";
        public Bitmap Image;
        public delegate void InvokeDelegate();
        public event InvokeDelegate OnSelect;

        public void Draw(DrawingContext context, Point Position)
        {

            context.DrawGeometry(brush, null, new EllipseGeometry(new Rect(Position, new Size(40, 40))));
            if (Image != null)
            {
                context.DrawImage(Image, new Rect(Position.X + 10, Position.Y + 10, 20, 20));
            }
        }

        public bool Hit(Point Mpos, Point Position)
        {
            var rect = new Rect(Position.X, Position.Y, 40, 40);
            return rect.Contains(Mpos);
        }

        public void Invoke()
        {
            OnSelect?.Invoke();
        }
    }
}
