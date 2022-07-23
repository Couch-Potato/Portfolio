using Avalonia;
using Avalonia.Media;
using Avalonia.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.AudioEditor
{
    public class Slider
    {
        public void Draw(DrawingContext context)
        {
            //Background
            context.DrawImage(Assets.BAR_BACKGROUND,  new Rect(0, 0, 12, 243), new Rect(X, YOffset, 12, 243));

            //Draw bg shade
            context.DrawGeometry(Brushes.Gray, null, new RectangleGeometry(new Rect(X, YOffset + (243 / 2) - 10, 12, -SliderValue + 10)));

            // Slider Item
            context.DrawImage(Assets.CIRCLE,  new Rect(0, 0, 20, 20), new Rect(X - 4, YOffset + (243 / 2) - 10 - SliderValue, 20, 20));

            var text = new FormattedText();
            text.Text = Label;
            text.Typeface = new Typeface(FontFamily.Default,  FontStyle.Normal, FontWeight.SemiBold);
            var xp = X - (text.Bounds.Width / 2) + 6;

            context.DrawText(Brushes.White, new Point(xp, YOffset + 243 + 10), text);
        }
        public string Label = "Volume";
        private bool Moving = false;
        public void MouseDown(Point mousepos)
        {

            var hitbox = new Rect(X - 4, YOffset + (243 / 2) - 10 - SliderValue, 20, 20);
            if (hitbox.Contains(mousepos))
            {

                Moving = true;
            }
        }
        public void MouseUp(Point mousepos)
        {
            Moving = false;
        }

        public void MouseMove(Point mousemov)
        {
            if (Moving)
            {
                SliderValue = MathUtilities.Clamp(-(int)mousemov.Y + YOffset + (243 / 2), (-243 / 2), (243 / 2));
            }
        }

        public Slider(int x, int y, string name)
        {
            Label = name;
            _x = x;
            YOffset = y;
        }
        public int SliderValue = 0;
        public int YOffset = 0;

        private int _x;
        public int X
        {
            get => _x;
        }
    }
}
