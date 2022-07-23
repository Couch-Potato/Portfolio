using Avalonia;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Novovu.Xenon.Assets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public class AssetItem
    {
        public int Measure = 1;
        public string Name = "test";
        public IBitmap Icon;
        public bool IsHovered = false;
        protected bool IsDrag = false;
        protected Point Offset;
        protected Point LastPoint;
        protected SolidColorBrush SecondaryColor = new SolidColorBrush(new Color(255, 44, 46, 68));
        protected SolidColorBrush BackgroundBrush = new SolidColorBrush(new Color(255,33,35,55));
        protected IControlAssetTab Parent;
        public delegate void Handler();
        public event Handler MouseUp;
        public Asset ItemLink { get; set; }

        public object Attachment { get; set; }

        public delegate void OnSelect();

        public event OnSelect Selected;
        public AssetItem(IControlAssetTab parent)
        {
            Parent = parent;
            Text.Typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.SemiBold);
            Text.FontSize = 12;
            Text.Constraint = new Size(76, 15);
            Text.TextAlignment = TextAlignment.Center;
            Text.TextWrapping = TextWrapping.WrapWithOverflow;
        }
        public Stopwatch sw = new Stopwatch();
        internal void MouseOneDown()
        {
            if (!new Rect(LastOrigin, new Size(80, 80)).Contains(MousePosition))
                return;
            if (!IsHovered)
                return;
            IsDrag = true;
            Offset = LastPoint - LastOrigin;
            
        }
        internal void MouseOneUp()
        {
            
            if (IsDrag)
            {
                Parent.CheckFolderCreation(this);
            }
            IsDrag = false;

            //We have clicked once
            if (sw.IsRunning && IsHovered)
            {
                if (sw.ElapsedMilliseconds > 250)
                {
                    //We are over the timelimit. Lets restart.
                    sw.Restart();
                }
                else if (IsHovered)
                {
                    sw.Reset();
                    Parent.InvokeInsert(this);
                    Selected?.Invoke();
                    
                }
            }else if (IsHovered)
            {
                //Start the timer and show we have clicked once.
                sw.Start();
            }
            MouseUp?.Invoke();
           
        }
        internal void MouseMove(Point p)
        {

            var rect = new Rect(LastOrigin, new Size(80, 80));
            IsHovered = rect.Contains(p);
            LastPoint = p;
        }

        protected Point MousePosition
        {
            get
            {
                if (Parent.Parent.Pointer != null)
                {
                    return new MouseDevice().GetPosition(Parent.Parent);
                }else
                {
                    return new Point(0,0);
                }
            }
        }
      
        protected Point LastOrigin;
        public void Render(DrawingContext context, Point Origin)
        {
            LastOrigin = Origin;
            var rect = new Rect(LastOrigin, new Size(80, 80));
            IsHovered = rect.Contains(LastPoint);
            if (IsDrag)
            {
                Origin = LastPoint - Offset;
            }
            
            //Draw outline of box
            var pen = new Pen(SecondaryColor, 2, DashStyle.Dash, PenLineCap.Round, PenLineJoin.Round);
            if (IsHovered)
            {
                pen.DashStyle = null;
                pen.Thickness = 3;
            }
            context.FillRectangle(BackgroundBrush, new Rect(Origin, new Size(80, 80)));
            context.DrawRectangle(pen, new Rect(Origin, new Size(80, 80)), 6);

            // Draw Image
            if (Icon != null)
            {
                context.DrawImage(Icon, new Rect(0, 0, 44, 44), new Rect(Origin + new Point(25, 9), new Size(44, 44)));
            }


            // Draw Text
           
            Text.Text = Name;
           
            context.DrawText(Brushes.White, new Point(Origin.X + 4, Origin.Y + 60), Text);

        }
        FormattedText Text = new FormattedText();
    }
}
