using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Novovu.Workshop.ViewModels;
using System.Threading.Tasks;

namespace Novovu.Workshop.Views
{
    public class ItemNameDialog : Window
    {
        public ItemNameDialog()
        {
            this.InitializeComponent();
            this.DataContext = new ItemNameDialogViewModel("");
            indvm = (ItemNameDialogViewModel)this.DataContext;//yes i know this is bad and I am lazy
            indvm.RequestClose += Indvm_RequestClose;
            this.SystemDecorations = SystemDecorations.BorderOnly;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        public ItemNameDialog(string name)
        {
            this.InitializeComponent();
            this.DataContext = new ItemNameDialogViewModel(name);
            indvm = (ItemNameDialogViewModel)this.DataContext;//yes i know this is bad and I am lazy
            indvm.RequestClose += Indvm_RequestClose;
            this.SystemDecorations = SystemDecorations.BorderOnly;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        private void Indvm_RequestClose(ItemNameDialogViewModel vm)
        {
            this.Close();
        }

        ItemNameDialogViewModel indvm;

        public new async Task<LoadDialogResponse> ShowDialog(Window parent)
        {
            await base.ShowDialog(parent);
            var ldr = new LoadDialogResponse()
            {
                DialogResult = indvm.Result,
                Value = indvm.Text
            };
            return ldr;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
