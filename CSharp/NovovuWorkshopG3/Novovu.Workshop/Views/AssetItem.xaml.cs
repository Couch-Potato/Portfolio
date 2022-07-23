using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Novovu.Workshop.ViewModels;

namespace Novovu.Workshop.Views
{
    public class AssetItem : UserControl
    {
  
        public AssetItem()
        {
            this.InitializeComponent();
            

        }
       
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
