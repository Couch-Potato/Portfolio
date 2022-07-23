using Avalonia.Media.Imaging;
using Novovu.Workshop.TypeInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Controls.AssetViewer
{
    public class ComponenetRepoAsset : AssetFolder
    {
        public WComponentRepo Repo;
        public ComponenetRepoAsset(IControlAssetTab parent, WComponentRepo linked) : base(parent)
        {
            Name = linked.Name;
            Repo = linked;
            Icon = new Bitmap("Assets/COMPONENT_REPO.png");
            foreach (var item in linked)
            {
                Items.Add(new ComponentAsset(ProjectModel.ProjectAssets.GetTab("Components"), item.Value));
            }
        }
        
    }
}
