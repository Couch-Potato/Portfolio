using Avalonia;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public interface IControlAssetTab
    {
        void Render(DrawingContext context, AssetViewer caller);
        void MouseOneDown();
        void MouseOneUp();
        void MouseMove(Point p);
        float TitleWidth { get; }
        bool IsVisible { get; set; }
        string TabName { get; set; }
        List<AssetItem> Items { get; set; }
        void InvokeInsert(AssetItem item);
        AssetViewer Parent { get; }
        
        event AssetTab.InsertDelegate ItemInsertRequested;

        void CheckFolderCreation(AssetItem caller);

        AssetFolder OpenedFolder { get; set; }

        bool Is(IControlAssetTab tab);
    }
}
