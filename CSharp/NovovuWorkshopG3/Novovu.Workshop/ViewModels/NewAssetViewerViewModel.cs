using Avalonia.Controls;
using Dock.Model.Controls;
using Novovu.Workshop.Controls.AssetViewer;
using Novovu.Workshop.DockSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class NewAssetViewerViewModel : Tool, IDockNameable, IViewAlias
    {
        public string Name => "Asset Viewer";
        AssetViewer av = new AssetViewer();
        public NewAssetViewerViewModel()
        {
            av.Tabs = ProjectModel.ProjectAssets.AssetTabs;
        }
        public IControl Get => av;
    }
}
