using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Novovu.Workshop.ViewModels;

namespace Novovu.Workshop.Views
{
    public class LoadDialog : Window
    {

        public LoadDialog()
        {
            this.InitializeComponent();
            this.SystemDecorations = SystemDecorations.BorderOnly;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.DataContext = new LoadDialogViewModel("Loading...");

        }
        public LoadDialog(string title)
        {
            this.InitializeComponent();
            this.SystemDecorations = SystemDecorations.BorderOnly;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.DataContext = new LoadDialogViewModel(title);


#if DEBUG
            //this.AttachDevTools();
#endif
        }

        public void SetStatus(string text, double percent)
        {
            ((LoadDialogViewModel)this.DataContext).ShowW = (443 / 100) * percent;
            ((LoadDialogViewModel)this.DataContext).Status = text;

        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
