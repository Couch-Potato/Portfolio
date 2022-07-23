using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Diagnostics;

namespace Novovu.Workshop.Views
{
    public class PropertyViewCategoryView : UserControl
    {
        public PropertyViewCategoryView()
        {
            this.InitializeComponent();
            this.InvalidateVisual();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
