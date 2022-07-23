using Dock.Model.Controls;
using Novovu.Workshop.Workspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class EngineControlViewModel : Tool, DockSystem.IDockNameable
    {
        public string Title = "Viewframe";
        public WorkspaceInterface WorkspaceInterface;

        public string Name => "Viewframe";
    }
}
