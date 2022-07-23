using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Novovu.Workshop.ViewModels;
using System;

namespace Novovu.Workshop.Views.Dialogs
{
    public class ErrorDialog : Window
    {
        public ErrorDialog()
        {
            this.InitializeComponent();
            
        }
        public ErrorDialog(Exception ex)
        {
            this.InitializeComponent();
            this.DataContext = new ErrorDialogViewModel(ex);
            this.Title = "Critical Error";
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
