using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Novovu.Workshop.Views
{
    public class MenuBar : UserControl
    {
        public MenuBar()
        {
            this.InitializeComponent();
            DataContext = new ViewModels.MenuBarViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
