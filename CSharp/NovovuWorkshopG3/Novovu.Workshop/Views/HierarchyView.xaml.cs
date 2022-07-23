using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Novovu.Workshop.ViewModels;

namespace Novovu.Workshop.Views
{
    public class HierarchyView : UserControl
    {
        public HierarchyView()
        {
            this.InitializeComponent();
            this.Find<TreeView>("tree").SelectionChanged += HierarchyView_SelectionChanged;
            
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var dxx = (HierarchyViewModel)DataContext;
            dxx.LinkedTree = this.Find<TreeView>("tree");
        }
        private void HierarchyView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dx = (HierarchyViewModel)DataContext;
            dx?.HandleSelect(sender, e);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
