using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public class AssetFolder : AssetItem
    {
        public List<AssetItem> Items = new List<AssetItem>();
        public AssetFolder(IControlAssetTab parent) : base(parent)
        {
            this.Name = "Folder";
          //  this.Icon = new Bitmap("Assets/Folder.png");
            this.Measure = 1;
            this.MouseUp += AssetFolder_MouseUp;
            Text.Typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.SemiBold);
            Text.FontSize = 12;
            Text.Constraint = new Size(76, 15);
            Text.TextAlignment = TextAlignment.Center;
            Text.TextWrapping = TextWrapping.WrapWithOverflow;
        }

        private void AssetFolder_MouseUp()
        {
            if (IsHovered)

            {
                Parent.OpenedFolder = Parent.OpenedFolder == this ? null : this;

            }

        }

        public new void MouseOneDown()
        {


            foreach (var item in Items)
            {
                item.MouseOneDown();
            }
            base.MouseOneDown();
        }
        public new void MouseOneUp()
        {
            foreach (var item in Items.ToArray())
            {
                item.MouseOneUp();
            }
            base.MouseOneUp();
        }
        public new void MouseMove(Point p)
        {
            //Debug.WriteLine("hey");
            //base.MouseMove(p);
            foreach (var item in Items)
            {
                item.MouseMove(p);
            }
            var rect = new Rect(LastOrigin, new Size(80, 80));


            IsHovered = rect.Contains(p);
        }
        public new void Render(DrawingContext context, Point Origin)
        {
            if (IsDrag)
            {
                Origin = LastPoint - Offset;
            }
            LastOrigin = Origin;
            //Draw outline of box
            var pen = new Pen(SecondaryColor, 2, DashStyle.Dash, PenLineCap.Round, PenLineJoin.Round);
            if (IsHovered)
            {
                pen.DashStyle = null;
                pen.Thickness = 3;
            }

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
