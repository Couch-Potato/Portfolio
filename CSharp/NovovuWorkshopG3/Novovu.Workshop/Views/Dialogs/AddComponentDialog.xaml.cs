using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Novovu.Workshop.Views.Dialogs
{
    public class AddComponentDialog : Window
    {
        public AddComponentDialog()
        {
            this.InitializeComponent();
#if DEBUG
            //this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
