using Avalonia;
using Avalonia.Media;
using Novovu.Workshop.Controls.AssetViewer;
using Novovu.Xenon.ScriptEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WComponentRepositories : ComponenetRepositories, IControlAssetTab
    {
        AssetTab tab;

        public event AssetTab.InsertDelegate ItemInsertRequested;

        public WComponentRepositories()
        {
            tab = new AssetTab();
            tab.TabName = "Components";
            tab.ItemInsertRequested += Tab_ItemInsertRequested;
        }

        private void Tab_ItemInsertRequested(AssetItem insertable)
        {
            ItemInsertRequested(insertable);
        }

        public float TitleWidth => tab.TitleWidth;

        public bool IsVisible { get => tab.isVisible; set => tab.isVisible = value; }
        public string TabName { get => tab.TabName; set => tab.TabName = value; }
        public List<AssetItem> Items { get => tab.Items; set => tab.Items=value; }

        public AssetViewer Parent =>tab.Parent;

        public AssetFolder OpenedFolder { get => tab.OpenedFolder; set => tab.OpenedFolder = value; }

        public void MouseMove(Point p)
        {
            tab.MouseMove(p);
        }

        public void MouseOneDown()
        {
            tab.MouseOneDown();
        }

        public void MouseOneUp()
        {
            tab.MouseOneUp();
        }

        public void Render(DrawingContext context, AssetViewer caller)
        {
            tab.Render(context, caller);
        }
        public void Add(string s, WComponentRepo repo)
        {
            base.Add(s, repo);
            tab.Items.Add(new ComponenetRepoAsset(tab, repo));
        }

        public void InvokeInsert(AssetItem item)
        {
            tab.InvokeInsert(item);
        }

        public void CheckFolderCreation(AssetItem caller)
        {
            tab.CheckFolderCreation(caller);
        }

        public bool Is(IControlAssetTab tab)
        {
            return (tab == this.tab);
        }
    }
}
