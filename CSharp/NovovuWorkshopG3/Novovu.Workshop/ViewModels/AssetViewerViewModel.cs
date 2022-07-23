using Novovu.Workshop.Models;
using Novovu.Workshop.Workspace;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Dock.Model.Controls;

namespace Novovu.Workshop.ViewModels
{
    public class AssetViewerViewModel : Tool, DockSystem.IDockNameable
    {
        public string Title = "Asset Viewer";

        WorkspaceInterface WSIF;
        public AssetViewerViewModel()
        {
            
            //Assets = new ObservableCollection<AssetCategory>(ProjectModel.ProjectAssets.GetProjectAssets());
        }

        public void AddAssetToCategory(string asset, AssetItem item)
        {
            foreach (var cat in Assets.Where(cat => cat.CategoryName == asset))
            {
                cat.Items.Add(item);
            }
        }

        

        public ObservableCollection<AssetCategory> Assets { get; }

        public string Name => "Assets";
    }
}
