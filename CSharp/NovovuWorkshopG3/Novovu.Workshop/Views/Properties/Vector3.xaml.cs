using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Novovu.Workshop.Views.Properties
{
    public class Vector3 : UserControl
    {
        public Vector3()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
