using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Novovu.Workshop.Views
{
    public class WorkspaceSelectorView : UserControl
    {
        public WorkspaceSelectorView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
