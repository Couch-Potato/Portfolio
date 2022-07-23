using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Novovu.Workshop.ViewModels;

namespace Novovu.Workshop.Views
{
    public class AssetViewerView : UserControl
    {
        public AssetViewerView()
        {
            this.InitializeComponent();
            //this.DataContext = new AssetViewerViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
