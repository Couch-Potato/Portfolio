using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public class AssetTab:IControlAssetTab
    {
        public string TabName = "Hello World";
        private AssetViewer viewer;
        internal bool isVisible = false;
        private List<AssetItem> _items = new List<AssetItem>();
        public List<AssetItem> Items { get => _items; set => _items=value; }
        public delegate void InsertDelegate(AssetItem insertable);
        public event InsertDelegate ItemInsertRequested;
        public AssetViewer Parent
        {
            get => viewer;
        }
        public void InvokeInsert(AssetItem item)
        {
            ItemInsertRequested?.Invoke(item);
        }
        private ISolidColorBrush TextBrush
        {
            get
            {
                if (isVisible)
                    return Brushes.White;
                else
                {
                    return new SolidColorBrush(new Color(255, 141, 142, 161));
                }
            }
        }
        private ISolidColorBrush UnderlineBrush
        {
            get
            {
                if (isVisible)
                    return Brushes.White;
                else
                {
                    return new SolidColorBrush(new Color(255, 33, 35, 55));
                }
            }
        }

        public AssetTab()
        {
            Text.Typeface = new Typeface(FontFamily.Default, FontStyle.Normal, FontWeight.Bold);
            Text.FontSize = 13;
        }
        public void CheckFolderCreation(AssetItem caller)
        {
            AssetItem converged = default;
            foreach (AssetItem item in Items)
            {
                if (item.IsHovered && item != caller)
                {
                    converged = item;
                }
            }
            if (converged != null)
            {
                if (converged is AssetFolder)
                {
                    // Folder
                    AssetFolder folder = (AssetFolder)converged;
                    folder.Items.Add(caller);
                    Items.Remove(caller);
                    folder.IsHovered = false;
                    caller.IsHovered = false;
                }
                else
                {
                    AssetFolder folder = new AssetFolder(this);
                    folder.Items.Add(converged);
                    folder.Items.Add(caller);
                    Items.Remove(caller);
                    Items.Remove(converged);
                    caller.IsHovered = false;
                    converged.IsHovered = false;
                    Items.Add(folder);
                }
            }
        }
        public AssetFolder OpenedFolder { get; set; }
        FormattedText Text = new FormattedText();
        public void Render(DrawingContext context, AssetViewer caller)
        {
            viewer = caller;
            //Draw object title
            
            Text.Text = TabName;
            

            
            context.DrawText(TextBrush, viewer.GetPositionForTabTitle(this), Text);

            //Underline
            context.FillRectangle(UnderlineBrush, new Rect(viewer.GetPositionForTabTitle(this) + new Point(0, 18), new Size(Text.Bounds.Width, 2)));

            float ItemAmt = 22f;
            float y = 50f;
            if (isVisible)
            {
                if (OpenedFolder != null)
                {
                    ItemAmt = 22f + 87f;

                    foreach (var item in OpenedFolder.Items)
                    {
                        if (ItemAmt + 22f + 80f > viewer.Width)
                        {
                            ItemAmt = 22f;
                            y += 90f;
                        }
                        item.Render(context, new Point(ItemAmt, y));
                        ItemAmt += (87f * item.Measure);
                    }
                    OpenedFolder.Render(context, new Point(22f, 50f));
                    return;
                }
                foreach (var item in Items)
                {
                    if (ItemAmt + 22f + 80f > viewer.Width)
                    {
                        ItemAmt = 22f;
                        y += 90f;
                    }
                    item.Render(context, new Point(ItemAmt, y));
                    ItemAmt += (87f * item.Measure);
                }
            }

        }
        public void MouseOneDown()
        {
            if (!isVisible)
                return;
            if (OpenedFolder != null)
            {
                OpenedFolder.MouseOneDown();
                return;
            }
            foreach (var item in Items)
            {
                item.MouseOneDown();
            }
        }
        public void MouseOneUp()
        {

            Text.Text = TabName;
            var rect = new Rect(viewer.GetPositionForTabTitle(this), Text.Bounds.Size);
            if (rect.Contains(Last))
            {
                isVisible = true;
                viewer.SetActive(this);
                return;
            }
            if (!isVisible)
                return;
            if (OpenedFolder != null)
            {
                OpenedFolder.MouseOneUp();
                return;
            }
            foreach (var item in Items.ToArray())
            {
                item.MouseOneUp();
            }
        }
        private Point Last;
        public void MouseMove(Point p)
        {
            Last = p;
            if (!isVisible)
                return;
            if (OpenedFolder != null)
            {
                OpenedFolder.MouseMove(p);
                return;
            }
            foreach (var item in Items)
            {
                item.MouseMove(p);
            }
        }
        public bool Hit(Point MousePosition)
        {
            return false;
        }

        public bool Is(IControlAssetTab tab)
        {
            return (tab == this);
        }

        public float TitleWidth
        {
            get
            {
                Text.Text = TabName;
                return (float)Text.Bounds.Width;
            }
        }

        public bool IsVisible { get => isVisible; set => isVisible = value; }
        string IControlAssetTab.TabName { get => TabName; set => TabName = value; }
    }
}
