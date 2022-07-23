using Avalonia;
using Avalonia.Media;
using Avalonia.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.KeyframeEditor
{
    public class KeyFrame
    {
        public float Size = 0f;
        public float Position = 0f;
        private float DRAG_OPACITY = 1f;
        private bool _drag = false;

        public KeyFrameData Data = new KeyFrameData();

        Layer ParentCached;
        private bool Drag
        {
            get => _drag;
            set
            {
                DRAG_OPACITY = value ? .8f : 1f;
                _drag = value;
            }
        }
        public void Render(DrawingContext context, Layer PARENT)
        {
            ParentCached = PARENT;
            var Y = PARENT.YValue;
            var HEIGHT = 24f;
            context.DrawImage(Assets.WHITE_BOX_EDGE,  new Rect(new Size(7, 64)), new Rect(
               new Point(127 + Position, Y),
               new Size(4, HEIGHT)
               ));
            context.DrawImage(Assets.WHITE_BOX_FILL,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 4 + Position, Y),
                new Size(20, HEIGHT)
                ));
            context.DrawImage(Assets.WHITE_BOX_EDGE_RIGHT,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 20 + 4 + Position, Y),
                new Size(4, HEIGHT)
                ));

            // Inner fill box
            context.DrawImage(Assets.KEYFRAME_INNER_EDGE, new Rect(new Size(7, 64)), new Rect(
               new Point(127 + 6 + Position, Y + 4),
               new Size(6, HEIGHT - 8)
               ));
            context.DrawImage(Assets.KEYFRAME_INNER_FILL,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 4 + 4 + Position, Y + 4),
                new Size(12, HEIGHT - 8)
                ));
            context.DrawImage(Assets.KEYFRAME_INNER_EDGE_RIGHT,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 12 + 4 + Position, Y + 4),
                new Size(6, HEIGHT - 8)
                ));
        }
        public void MouseDown(Point MousePosition, KeyFrameEditor editor)
        {

            if (HitBox(MousePosition))
                Drag = true;
        }
        public void MouseUp(Point MousePosition, KeyFrameEditor editor)
        {
            if (Drag)
                Drag = false;
        }

        public void DragMoved(Point MousePosition, KeyFrameEditor editor)
        {
            if (!Drag)
                return;
            Position = (float)MathUtilities.Clamp(
                MousePosition.X - 127,
                0f,
                690f - 29f
                );
        }

        private bool HitBox(Point pos)
        {
            var Y = ParentCached.YValue;
            var rect1 = new Rect(
                new Point(127 + 4 + 4 + Position, Y + 4),
                new Size(12, 24f - 8)
                );
            return rect1.Contains(pos);
        }
    }
}
