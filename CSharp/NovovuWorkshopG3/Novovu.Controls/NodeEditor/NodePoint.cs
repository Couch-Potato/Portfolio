using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Novovu.Controls.NodeEditor
{
    public class NodePoint
    {
        public NodePoint(string name, string type, int pos, NodeBox parent, PointFlowTypes typex, object defaultval = null)
        {
            Name = name;
            Type = type;
            IndexPosition = pos;
            Parent = parent;
            PointFlow = typex;
            StaticAssets.Load();
            form.Typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Medium);
            form.FontSize = 12;
            _generated = defaultval;
        }
        public NodePoint()
        {

        }

        private object _generated = default;

        internal object GeneratedValue
        {
            get => _generated;
            set
            {
                _generated = value;
                GeneratedValueChanged?.Invoke(this, new EventArgs());
            }
        }

        public T Get<T>()
        {
            return (T)GeneratedValue;
        }

        public string Name;
        public string Type;
        [XmlIgnoreAttribute]
        public NodeBox Parent;
        public int IndexPosition;
        public NodeConnection Connection;

        public event EventHandler GeneratedValueChanged;

        public Point ConnectionPoint
        {
            get
            {
                var apos = Parent.GetPositionFromIndex(this);
                if (PointFlow == PointFlowTypes.Input)
                {

                    return apos + new Point(7, 7);
                }
                else
                {
                    return new Point(apos.X + Measure().Width - 17, apos.Y + 7);

                }

            }
        }
        public enum PointFlowTypes { Input, Output }
        public PointFlowTypes PointFlow = PointFlowTypes.Input;
        public void Unhook()
        {
            if (Connection != null)
            {
                Connection.Input = null;
                Connection.Output = null;
                Connection = null;
            }
        }
        public void Click(Point mousepos, VectorCamera cam)
        {
            var apos = Parent.GetPositionFromIndex(this) + cam.Position;
            var detector = new Rect(apos.X, apos.Y, 14, 14);
            if (PointFlow == PointFlowTypes.Output)
            {
                detector = new Rect(apos.X + form.Bounds.Width + 10, apos.Y, 14, 14);
            }
            if (detector.Contains(mousepos))
            {
                ConnectionFactory.Connect(this);
            }
        }
        FormattedText form = new FormattedText();
        
               
        public void Draw(DrawingContext context, VectorCamera cam)
        {
            var apos = Parent.GetPositionFromIndex(this) + cam.Position;
            form.Text = Name;
            if (PointFlow == PointFlowTypes.Input)
            {
                context.DrawText(Brushes.White, new Point(apos.X + 22, apos.Y - 2), form);
                var img = StaticAssets.ConnectionUnfilled;
                if (Connection != null)
                {
                    img = StaticAssets.ConnectionFilled;
                    Connection.Draw(context, cam);
                }
                context.DrawImage(img, new Rect(0, 0, 22, 22), new Rect(apos.X, apos.Y, 14, 14));
            }
            else
            {
               
                context.DrawText(Brushes.White, new Point(apos.X, apos.Y - 2), form);
                var img = StaticAssets.ConnectionUnfilled;
                if (Connection != null)
                {
                    img = StaticAssets.ConnectionFilled;
                    Connection.Draw(context, cam);
                }
                context.DrawImage(img, new Rect(0, 0, 22, 22), new Rect(apos.X + form.Bounds.Width + 10, apos.Y, 14, 14));
            }
        }
        public Size Measure()
        {
            form.Text = Name;
            return new Size(14 + 22 + form.Bounds.Width, 14);
        }


    }
}
