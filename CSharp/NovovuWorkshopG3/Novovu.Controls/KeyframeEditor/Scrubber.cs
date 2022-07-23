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
    public class Scrubber
    {
        public float Position = 100f;
        private float DRAG_OPACITY = 1f;
        private bool _drag = false;
        private bool Drag
        {
            get => _drag;
            set
            {
                DRAG_OPACITY = value ? .8f : 1f;
                _drag = value;
            }
        }
        public void Render(DrawingContext context, KeyFrameEditor editor)
        {
            context.DrawImage(Assets.WHITE_BOX_EDGE,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + Position, 31),
                new Size(7, 7)
                ));
            context.DrawImage(Assets.WHITE_BOX_FILL,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 7 + Position, 31),
                new Size(14, 7)
                ));
            context.DrawImage(Assets.WHITE_BOX_EDGE_RIGHT, new Rect(new Size(7, 64)), new Rect(
                new Point(127 + Position + 14 + 7, 31),
                new Size(7, 7)
                ));

            // White box going down

            context.DrawImage(Assets.WHITE_BOX_FILL, new Rect(new Size(7, 64)), new Rect(
                new Point(127 + Position + 13, 31),
                new Size(2, 1080)
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
        public delegate void ScrubberMoveDelegate(float position);
        public event ScrubberMoveDelegate ScrubberMoved;

        public void DragMoved(Point MousePosition, KeyFrameEditor editor)
        {
            if (!Drag)
                return;
            Position = (float)MathUtilities.Clamp(
                MousePosition.X - 127,
                0f,
                690f - 29f
                );
            ScrubberMoved?.Invoke(Position);
        }

        private bool HitBox(Point pos)
        {
            var rect1 = new Rect(
                new Point(127 + Position + 14 + 7, 31),
                new Size(7, 7));
            var rect2 = new Rect(
                new Point(127 + 7 + Position, 31),
                new Size(14, 7)
                );
            var rect3 = new Rect(
                new Point(127 + Position, 31),
                new Size(7, 7)
                );
            return rect1.Contains(pos) || rect2.Contains(pos) || rect3.Contains(pos);
        }
    }
}
