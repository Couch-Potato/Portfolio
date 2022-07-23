using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Models
{
    public abstract class ToolItem
    {
        IAssetLoader assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
        private Bitmap Icon;
        private Bitmap SelectedIcon;
        public delegate void SelectionUpdate();

        public SelectionUpdate SelectionGained;
        public SelectionUpdate SelectionLost;

        public bool Selected = false;
        public Bitmap ItemMap
        {
            get => Selected ? SelectedIcon : Icon;
        }
        public ToolItem(string Icon, string selected)
        {
            this.Icon = new Bitmap(assets.Open(new Uri("avares://Novovu.Workshop/Assets/" + Icon)));
            this.SelectedIcon = new Bitmap(assets.Open(new Uri("avares://Novovu.Workshop/Assets/" + selected)));
        }
    }
}
