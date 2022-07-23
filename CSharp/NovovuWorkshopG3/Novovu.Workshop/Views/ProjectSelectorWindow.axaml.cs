using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Novovu.Workshop.Views
{
    public class ProjectSelectorWindow : Window
    {
        public ProjectSelectorWindow()
        {
            this.InitializeComponent();

        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
