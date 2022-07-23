using System;
using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Dock.Model;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using ReactiveUI;

namespace Novovu.Workshop
{
    public class ViewLocator : IDataTemplate
    {
        public bool SupportsRecycling => false;

        public IControl Build(object data)
        {
            var name = data.GetType().FullName.Replace("ViewModel", "View");
            if (name == "Novovu.Workshop.Views.EngineControlView")
            {
                return Engine.ControlPlane;
            }
                
            if (name == "Novovu.Workshop.Views.KeyFrameView")
                return Engine.GenerateKeyFrameEditor();
            var type = Type.GetType(name);

            if (data is IViewAlias)
            {
                return ((IViewAlias)data).Get;
            }

            if (type != null)
            {
                if (type == typeof(CodeEditorView))
                {
                    return (Control)Activator.CreateInstance(type, data);
                }
                var cx = (Control)Activator.CreateInstance(type);
                
                return cx;
            }
            else
            {
                var n2 = data.GetType().FullName.Replace("ViewModel", "");
                var type2 = Type.GetType(n2);
                var cx = (Control)Activator.CreateInstance(type2);
                cx.DataContext = data;
                return cx;

               // return new TextBlock { Text = "Not Found: " + name + " or " + n2 };
            }
        }

        public bool Match(object data)
        {
            return data is ReactiveObject || data is IDock;
        }
    }
}