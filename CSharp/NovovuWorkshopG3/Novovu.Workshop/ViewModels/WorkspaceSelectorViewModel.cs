
using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.ViewModels
{
    public class WorkspaceSelectorViewModel:ViewModelBase
    {
        public string Username => ProjectModel.ProjectStatic.LoggedInUsername;
        public IBitmap Avatar => new Bitmap(ProjectModel.ProjectStatic.LoggedInUserImage);

        public string UserDesc => ProjectModel.ProjectStatic.LoggedInUserDescription;

        private Workspace.Workspace _s;
        public Workspace.Workspace Selected { get => _s; set => this.RaiseAndSetIfChanged(ref _s, value); }

        public int _cw = 0;
        public int CollapseWidth { get => _cw; set => this.RaiseAndSetIfChanged(ref _cw, value); }

        public ObservableCollection<Workspace.Workspace> Workspaces { get; }
        private MainWindowViewModel MainWindowViewModel;

        public WorkspaceSelectorViewModel(MainWindowViewModel mvm)
        {
            Workspace.WorkspaceRegistry.ConstructWorkspaces();
            Workspaces = new ObservableCollection<Workspace.Workspace>(Workspace.WorkspaceRegistry.Workspaces);
            Selected = Workspaces[0];
            MainWindowViewModel = mvm;
            MainWindowViewModel.SelectedWS = Selected;
            Selected.Activate();
            Workspace.WorkspaceRegistry.ActivationChanged += WorkspaceRegistry_ActivationChanged;
        }

        private void WorkspaceRegistry_ActivationChanged(Workspace.Workspace w)
        {
            Selected = w;
            CollapseWidth = 0;
        }

        public void MenuHandle()
        {
            if (CollapseWidth == 0)
            {
                CollapseWidth = 1920;
            }else
            {
                CollapseWidth = 0;
            }
        }
    }
}
