using Avalonia;
using Avalonia.Media;
using Avalonia.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.NodeEditor.DefaultNodes
{
    public class IntensitySlider
    {
        public IntensitySlider(IntensityNode inx)
        {
            IntensityNode = inx;
        }
        private IntensityNode IntensityNode;
        public float Value => SliderPos / 100f;
        internal float SliderPos = 50f;
        private bool IsDragging = false;
        public void Draw(DrawingContext context, Point Positon)
        {
            // Outline rectangle
            context.DrawGeometry(Brushes.Gray, new Pen(), new RectangleGeometry(new Rect(Positon, new Size(100, 10))));

            // Outline shaded
            context.DrawGeometry(Brushes.DarkGray, new Pen(), new RectangleGeometry(new Rect(Positon, new Size(SliderPos, 10))));

            // Outline Slider
            context.DrawGeometry(Brushes.White, new Pen(), new RectangleGeometry(new Rect(Positon + new Point(SliderPos, -2), new Size(5, 14))));
        }
        public bool DragBegin(Point MousePosition, Point Positon)
        {
            if (new Rect(Positon + new Point(SliderPos, -2), new Size(5, 14)).Contains(MousePosition))
            {
                IsDragging = true;
                return true;
            }
            return false;
        }
        public void DragEnd()
        {
            IsDragging = false;
        }
        public void MouseMoved(Point MousePosition, Point Positon)
        {
            if (IsDragging)
            {
                SliderPos = (float)MathUtilities.Clamp((MousePosition - Positon).X, 0, 100);
                IntensityNode.SliderValue = Value;
            }
        }
    }
}
