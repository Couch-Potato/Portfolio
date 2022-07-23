using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Novovu.Controls.NodeEditor
{
    public class NodeBox
    {
        public Point Position = new Point(0, 0);

        public Size Size = new Size(200, 300);

        private Bitmap top_left;
        private Bitmap top_center;
        private Bitmap top_right;
        private Bitmap bottom_left;
        private Bitmap bottom_right;
        private Bitmap center;
        private Bitmap close_on;
        private Bitmap close_off;
        private Rect src = new Rect(0, 0, 22, 22);


        public delegate void PaintCustom(DrawingContext context, VectorCamera cam);
        public event PaintCustom ReadyFeaturePaint;
        public delegate bool MouseEvent(Point MousePosition, VectorCamera cam);

        public MouseEvent RequestDrag;
        public MouseEvent Dragged;
        public MouseEvent DragEnd;



        public float NodeYOffset = 0f;

        private string _title = "Phong Shader";
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Measure();
            }
        }
        private string _type = "Technique";
        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                Measure();
            }
        }
        [XmlElementAttribute()]
        private bool ExitHover = false;

        public List<NodePoint> Inputs = new List<NodePoint>();

        public List<NodePoint> Outputs = new List<NodePoint>();
        public NodeBox()
        {
            var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();


            top_left = new Bitmap(File.OpenRead("Assets/dark_left.png"));
            top_center = new Bitmap(File.OpenRead("Assets/dark_center.png"));
            top_right = new Bitmap(File.OpenRead("Assets/dark_right.png"));
            bottom_left = new Bitmap(File.OpenRead("Assets/main_left.png"));
            bottom_right = new Bitmap(File.OpenRead("Assets/main_right.png"));
            center = new Bitmap(File.OpenRead("Assets/main_center.png"));
            close_off = new Bitmap(File.OpenRead("Assets/cirlce_dark.png"));
            close_on = new Bitmap(File.OpenRead("Assets/circle_on.png"));


           
            formx.Typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Bold);
            formx.FontSize = 12;
            form.Typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Medium);
            form.FontSize = 12;
        }
        public void Measure()
        {

        }
        public Point GetPositionFromIndex(NodePoint c)
        {
            if (c.PointFlow == NodePoint.PointFlowTypes.Input)
            {
                var PointOffset = new Point(10, 44 + NodeYOffset);
                PointOffset += Position;
                PointOffset += new Point(0, 24 * c.IndexPosition);
                return PointOffset;
            }
            else
            {
                var size = c.Measure();
                var PointOffset = new Point(0, 44 + NodeYOffset);
                PointOffset += Position;
                PointOffset += new Point(Size.Width - size.Width, 24 * c.IndexPosition);
                return PointOffset;
            }

        }
        public void Draw(DrawingContext context, VectorCamera cam)
        {

            var adjpos = Position + cam.Position;
            //Draw the top portion;
            context.DrawImage(top_left,  src, ptr(new Point(0 + adjpos.X, 0 + adjpos.Y)));
            context.DrawImage(top_center,  src, new Rect(22 + adjpos.X, 0 + adjpos.Y, Size.Width - 44, 22));
            context.DrawImage(top_right,  src, ptr(new Point(Size.Width - 22 + adjpos.X, 0 + adjpos.Y)));

            //Draw center portion
            context.DrawImage(center,  src, new Rect(0 + adjpos.X, 22 + adjpos.Y, Size.Width, Size.Height - 44));

            var ny = Size.Height - 22 + adjpos.Y;
            context.DrawImage(bottom_left,  src, new Rect(0 + adjpos.X, ny, 22, 22));
            context.DrawImage(center,  src, new Rect(22 + adjpos.X, ny, Size.Width - 44, 22));
            context.DrawImage(bottom_right,  src, new Rect(Size.Width - 22 + adjpos.X, ny, 22, 22));

            //Draw the text
            form.Text = Title;
       
            context.DrawText(Brushes.White, adjpos + new Point(6, 2), form);

            //Draw the type
            formx.Text = Type.ToUpper();
            context.DrawText(Brushes.DarkGray, adjpos + new Point(form.Bounds.Width + 6 + 10, 3), formx);

            //Draw the exit button
            var img = ExitHover ? close_on : close_off;
            var widthx = Size.Width - 22 - 10 + adjpos.X;

            context.DrawImage(img, new Rect(0, 0, 22, 22), new Rect(widthx, adjpos.Y + 3, 16, 16));
            foreach (NodePoint point in Inputs)
            {
                point.Draw(context, cam);
            }
            foreach (NodePoint point in Outputs)
            {
                point.Draw(context, cam);
            }
            ReadyFeaturePaint?.Invoke(context, cam);
        }
        FormattedText formx = new FormattedText();
        FormattedText form = new FormattedText();
      
        [XmlIgnoreAttribute]
        public bool Drag = false;
        public delegate void RemovalRequestHandler(NodeBox box);

        public event RemovalRequestHandler OnRemove;
        public bool IsMouseOverHeader(Point mousePos, VectorCamera cam)
        {
            var adjpos = Position + cam.Position;
            var exitrect = new Rect(Size.Width - 22 - 10 + adjpos.X, adjpos.Y + 3, 16, 16);
            if (exitrect.Contains(mousePos))
            {
                OnRemove?.Invoke(this);
                foreach (var x in Inputs)
                {
                    x.Connection?.Terminate();
                }
                foreach (var x in Outputs)
                {
                    x.Connection?.Terminate();
                }
            }
            var rect1 = ptr(new Point(0 + adjpos.X, 0 + adjpos.Y));
            var rect2 = new Rect(22 + adjpos.X, 0 + adjpos.Y, Size.Width - 44, 22);
            var rect3 = ptr(new Point(Size.Width - 22 + adjpos.X, 0 + adjpos.Y));
            return rect1.Contains(mousePos) || rect2.Contains(mousePos) || rect3.Contains(mousePos);
        }
        public void Click(Point mousePos, VectorCamera cam)
        {
            //Check if the user clicked on this box
            Rect c2r = new Rect(Position + cam.Position, Size);
            if (c2r.Contains(mousePos))
            {
                foreach (var point in Inputs)
                {
                    point.Click(mousePos, cam);
                }
                foreach (var point in Outputs)
                {
                    point.Click(mousePos, cam);
                }
            }
        }
        public void Hover(Point mousePos, VectorCamera cam)
        {
            var adjpos = Position + cam.Position;
            var widthx = Size.Width - 22 - 10 + adjpos.X;
            var exitrect = new Rect(widthx, adjpos.Y + 3, 16, 16);
            ExitHover = exitrect.Contains(mousePos);
        }
        private Rect ptr(Point p)
        {
            return new Rect(p.X, p.Y, src.Width, src.Height);
        }
        bool AnyDragListeners = false;
        public bool DragBegin(Point p, VectorCamera cam)
        {
            if (RequestDrag != null)
            {
                AnyDragListeners = RequestDrag(p, cam);
                return AnyDragListeners;
            }
            return false;

        }
        public bool DraggedMouse(Point p, VectorCamera cam)
        {
            if (AnyDragListeners)
            {
                Dragged?.Invoke(p, cam);
                return true;
            }
            return false;
        }
        public bool DragStop(Point p, VectorCamera cam)
        {
            if ((bool)AnyDragListeners)
            {
                DragEnd?.Invoke(p, cam);
                AnyDragListeners = false;
            }
            return true;
        }
    }
}
