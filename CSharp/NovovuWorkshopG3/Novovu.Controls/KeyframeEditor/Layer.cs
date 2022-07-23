using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.KeyframeEditor
{
    public class Layer
    {
        //DESIGN PARAMS

        private const float Y_OFFSET = 4f;
        public const float HEIGHT = 24f;



        public string Name = "OBJECT NAME";
        public string Icon;
        public Bitmap IconImage;
        public List<KeyFrame> Keyframes = new List<KeyFrame>();
        private KeyFrameEditor ParentedEditor;
        public void Render(DrawingContext context, KeyFrameEditor editor)
        {
            ParentedEditor = editor;
            var Index = editor.GetIndexOfLayer(this);
            var Y = (Index * (HEIGHT + Y_OFFSET)) + Y_OFFSET + 38;

            // Render the title
            FormattedText test = new FormattedText();
            test.Text = Name;
            test.FontSize = 12;
            test.Typeface = new Typeface(FontFamily.Default,  FontStyle.Normal, FontWeight.SemiBold);
            context.DrawText(Brushes.White, new Point(122 - test.Bounds.Width, Y + 3), test);

            context.DrawImage(Assets.SCRUBBER_BOX_EDGE,  new Rect(new Size(7, 64)), new Rect(
               new Point(127, Y),
               new Size(4, HEIGHT)
               ));
            context.DrawImage(Assets.SCRUBBER_BOX_FILL,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 4, Y),
                new Size(690 - 8, HEIGHT)
                ));
            context.DrawImage(Assets.SCRUBBER_BOX_EDGE_RIGHT,  new Rect(new Size(7, 64)), new Rect(
                new Point(127 + 690 - 4, Y),
                new Size(4, HEIGHT)
                ));

            // render the intermediate boxes
            for (int i = 0; i < Keyframes.Count - 1; i++)
            {
                context.DrawImage(Assets.KEYFRAME_INNER_FILL,  new Rect(new Size(7, 64)), new Rect(
                new Point(Keyframes[i].Position + 10 + 127, Y),
                new Size(Keyframes[i + 1].Position - Keyframes[i].Position, HEIGHT)
                ));
            }
            foreach (var frame in Keyframes)
            {
                frame.Render(context, this);
            }

        }
        public void MouseDown(Point MousePosition, KeyFrameEditor editor)
        {
            foreach (var kf in Keyframes)
            {
                kf.MouseDown(MousePosition, editor);
            }
        }
        public void MouseUp(Point MousePosition, KeyFrameEditor editor)
        {
            foreach (var kf in Keyframes)
            {
                kf.MouseUp(MousePosition, editor);
            }
        }

        public void DragMoved(Point MousePosition, KeyFrameEditor editor)
        {
            foreach (var kf in Keyframes)
            {
                kf.DragMoved(MousePosition, editor);
            }
        }

        public float YValue
        {
            get
            {
                var Index = ParentedEditor.GetIndexOfLayer(this);
                var Y = (Index * (HEIGHT + Y_OFFSET)) + Y_OFFSET + 38;
                return Y;
            }
        }
    }
}
