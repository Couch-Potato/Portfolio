using Avalonia;
using Avalonia.Media;
using Novovu.Controls.Animation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Controls.ContextMenu
{
    public class ContextMenu
    {
        public Brush brush = new SolidColorBrush(new Color(255, 33, 35, 55));
        public List<ContextMenuItem> Items = new List<ContextMenuItem>() {};
        public Animator MenuAnimator = new Animator();
        public Point invpos = new Point(100, 100);
        public Point CalculatePosition(ContextMenuItem item)
        {
            var index = Items.IndexOf(item);
            var offset = (Items.Count - 1) * 22;
            return invpos - new Point(-offset + (index * 44), 45);

        }
        public ContextMenu()
        {
        }
        bool Visible = false;
        bool Hiding = false;
        bool Showing = false;
        public void Show()
        {
            Visible = false;
            Showing = true;
            sw.Restart();
        }
        public void Hide()
        {
            if (Visible == false)
                return;
            Visible = false;
            Hiding = true;
            sw.Restart();
        }
        Stopwatch sw = new Stopwatch();
        public void Draw(DrawingContext context)
        {
            if (Showing)
            {
                float s = sw.ElapsedMilliseconds / 1000f;
                if (s >= .6)
                {
                    Visible = true;
                    Showing = false;
                }
                else
                {

                    foreach (ContextMenuItem item in Items)
                    {
                        var z = CalculatePosition(item);
                        item.Draw(context, Animator.AnimateQuart((float)s, .6f, invpos, z));
                    }
                    context.DrawGeometry(brush, null, new EllipseGeometry(new Rect(invpos + new Point(5, 0), new Size(30, 30))));
                }
            }

            if (Visible)
            {
                foreach (ContextMenuItem item in Items)
                {
                    var pos2 = CalculatePosition(item);

                    item.Draw(context, CalculatePosition(item));
                }
                context.DrawGeometry(brush, null, new EllipseGeometry(new Rect(invpos + new Point(5, 0), new Size(30, 30))));
                if (Hovered != null && Hovered.Image != null)
                {
                    context.DrawImage(Hovered.Image, new Rect(invpos + new Point(12, 7), new Size(16, 16)));
                }
                
                if (!string.IsNullOrEmpty(HoverText))
                {
                    /*FormattedText text = new FormattedText();
                    text.Typeface = tf;
                    text.FontSize = 14;
                    text.Constraint = new Size(100, 20);
                    text.TextAlignment = TextAlignment.Center;
                    text.Text = HoverText;
                    var pt = invpos - new Point(25, -30);*/
                }

            }
            if (Hiding)
            {
                float s = sw.ElapsedMilliseconds / 1000f;
                // This is lower than the animation time of .6s as we want it to disappear before the animation is completed.
                if (s >= .3)
                {
                    Visible = false;
                    Hiding = false;
                    Showing = false;
                }
                else
                {

                    foreach (ContextMenuItem item in Items)
                    {
                        var z = CalculatePosition(item);
                        item.Draw(context, Animator.AnimateQuart((float)s, .6f, z, invpos));
                    }
                    context.DrawGeometry(brush, null, new EllipseGeometry(new Rect(invpos + new Point(5, 0), new Size(30, 30))));
                }
            }

        }
        Typeface tf = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.SemiBold);
        string HoverText = "";
        ContextMenuItem Hovered;
        public void MouseMove(Point position)
        {
            HoverText = "";
            var ix = GetHoveredItem(position);
            Hovered = ix;
            if (ix != null)
            {
                HoverText = ix.Name;
            }
            else
            {

            }
        }
        public void Click(Point position)
        {
            var ix = GetHoveredItem(position);
            Hide();
            ix?.Invoke();
        }


        public ContextMenuItem GetHoveredItem(Point mousepos)
        {
            foreach (ContextMenuItem item in Items)
            {
                if (item.Hit(mousepos, CalculatePosition(item)))
                {
                    return item;
                }
            }
            return null;
        }

        public void InvokePosition(ContextMenuItem item)
        {
            item.Invoke();
        }
    }
}
