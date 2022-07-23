using Novovu.Workshop.Controls.AssetViewer;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Workshop.Workspace;
using Novovu.Xenon.ScriptEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WComponentRepo : ComponentRepository, ITreeItem, IContextable, Models.IIconable
    {
        public string TreeItemName { get; }
        public WComponentRepo(string repoName, bool ReadOnly = true, bool AllowDelete = false, ComponentRepository linked = null)
        {

            Name = repoName;
            TreeItemName = repoName;
            Children = new ObservableCollection<ITreeItem>();
            if (ReadOnly)
            {
                var AddRepo = new ContextOption("Add Repository");
                AddRepo.ContextClick += AddRepo_ContextClick;
                ContextOptions = new ContextOption[] {AddRepo };
            }
            else
            {
                var AddComponent = new ContextOption("Add Component");
                var AddRepo = new ContextOption("Add Repository");
                var Delete = new ContextOption("Delete Repository");
                Delete.ContextClick += Delete_ContextClick;
                AddRepo.ContextClick += AddRepo_ContextClick;
                AddComponent.ContextClick += AddComponent_ContextClick;
                if (AllowDelete)
                {
                    ContextOptions = new ContextOption[] {
                        Delete,
                    AddComponent,
                    AddRepo
                };
                }else
                {
                    ContextOptions = new ContextOption[] {
                    AddComponent,
                    AddRepo
                };
                }
            }
            
        }
        public static WComponentRepo FromExistingRepo(ComponentRepository repo, bool ReadOnly = true, bool Allowdelete = false)
        {
            var repex =  new WComponentRepo(repo.Name, ReadOnly, Allowdelete, repo);
            foreach (var comp in repo)
            {
                Debug.WriteLine(comp.Key);
                repex.Add(comp.Value);
                //repex.Children.Add(new WCoreComponent(comp.Value));
            }
            return repex;
        }
        private void Delete_ContextClick()
        {
            Workspace.ScriptEditor.Repositories.Remove(this);
            Workspace.ScriptEditor.Hierarchy.GameObjects.Remove(this);

        }

        private async void AddRepo_ContextClick()
        {
            LoadDialogResponse response = await new ItemNameDialog("Enter Repository Name").ShowDialog(ProjectStatic.MainWindow);

            if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
            {
                WComponentRepo rep = new WComponentRepo(response.Value, false, true);
                Workspace.ScriptEditor.Repositories.Add(rep);
                Workspace.ScriptEditor.Hierarchy.GameObjects.Add(rep);
            }
            
        }

        private async void AddComponent_ContextClick()
        {
            LoadDialogResponse response = await new ItemNameDialog("Enter Component Name").ShowDialog(ProjectStatic.MainWindow);

            if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
            {
                var comp = new WComponent(response.Value, false, this);
                Children.Add(comp);
                Add(comp);
                var tab = ProjectAssets.GetTab("Components");
                foreach (var item in tab.Items)
                {
                    if (item is ComponenetRepoAsset)
                        if (((ComponenetRepoAsset)item).Repo == this)
                        {
                            var x = (ComponenetRepoAsset)item;
                            x.Items.Add(new ComponentAsset(tab, comp));
                        }
                }
            }
        }

        public string IconSource => "avares://Novovu.Workshop/Assets/COMPONENT_REPO.png";
        public ObservableCollection<ITreeItem> Children { get; }

        public ContextOption[] ContextOptions { get; }

    }
}
