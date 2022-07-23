using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Novovu.Workshop.Views
{
    public class PropertiesView : UserControl
    {
        public PropertiesView()
        {
            this.InitializeComponent();
            
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
