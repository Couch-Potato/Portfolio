using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class MenuBarViewModel : ViewModelBase
    {
        public void RunLevel()
        {
            Workspace.WorkspaceRegistry.GetWorkspace<Workspace.LevelDesigner>().RunGame();
        }

      /*  THERE HAS TO BE A MUCH BETTER WAY TO DO THIS AND UNFORTUNATEY
        I HAVE NO TIME NORE WILL TO FIND THAT OUT.*/
        public void ImportLevel()
        {

        }

        public void ExportLevel()
        {

        }

        public void Import()
        {
            ///IMPORT HERE
        }

        public void InsertLight()
        {

        }


        public void RenderSelectedObject()
        {

        }
        public void Render()
        {

        }
        private bool _wireframe = false;
        public bool Wireframe { get => _wireframe; set {
                this.RaiseAndSetIfChanged(ref _wireframe, value);
            } }
        public void ToggleWireframe()
        {

        }

        public void ViewObjectDesigner()
        {

        }

        public void ViewLevelEditor()
        {

        }

        public void ViewScriptEditor()
        {

        }
    }
}
