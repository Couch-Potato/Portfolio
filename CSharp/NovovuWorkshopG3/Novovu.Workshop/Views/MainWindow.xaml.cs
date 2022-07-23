using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform;
using Avalonia.VisualTree;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Views.Dialogs;
using Novovu.Workshop.VSCode;
using System;
using System.IO;

namespace Novovu.Workshop.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            //this.SystemDecorations = SystemDecorations.None;
            Novovu.Xenon.AvaloniaControl.GlobalWindowHost.Handle = ((IWindowImpl)((TopLevel)this.GetVisualRoot())?.PlatformImpl)?.Handle?.Handle ?? IntPtr.Zero;
            ProjectStatic.MainWindow = this;
            InitializeComponent();
            ProjectModel.DiscordRPC.Initialize();
            Engine.GenerateControlPlane();

            this.Renderer.DrawFps = true;
            //new Dialogs.ErrorDialog(new IOException()).ShowDialog(this);
            this.Closed += MainWindow_Closed;
          
#if DEBUG
            //this.AttachDevTools();
#endif
            //new AddComponentDialog().ShowDialog(this);
        }

      

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitializeComponent()
        {
     
                AvaloniaXamlLoader.Load(this);
         
           
        }
    }
}
