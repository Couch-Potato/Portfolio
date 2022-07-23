using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Workshop.Workspace;
using Novovu.Xenon.ScriptEngine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WScript : Script, ITreeItem, IContextable, Models.IIconable
    {
        public ContextOption[] ContextOptions { get; private set; }

        public WComponent Parent;


        public WScript(string componentName, bool isReadOnly, WComponent parent = null)
        {
            Name = componentName;
            if (!isReadOnly)
            {
                var OpenScript = new ContextOption("Open");
                OpenScript.ContextClick += OpenScript_ContextClick;
                var Delete = new ContextOption("Delete");
                Delete.ContextClick += Delete_ContextClick1;
                ContextOptions = new ContextOption[]
                {
                    OpenScript, Delete
                };
            }
            Parent = parent;
        }

        private void Delete_ContextClick1()
        {
            throw new NotImplementedException();
        }

        private void OpenScript_ContextClick()
        {
            Workspace.WorkspaceRegistry.GetWorkspace<ScriptEditor>().OpenCode(Name, Source, this);
        }

        private async void Rename_ContextClick()
        {
            LoadDialogResponse response = await new ItemNameDialog("Enter Component").ShowDialog(ProjectStatic.MainWindow);

            if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
            {
                TreeItemName = response.Value;
            }
            Workspace.ScriptEditor.RebuildHierarchy();
        }

        private void Delete_ContextClick()
        {
            if (Parent != null)
            {
                Parent.Children.Remove(this);
            }
        }
        public WScript()
        {

        }
        private async void AddScript_ContextClick()
        {
            LoadDialogResponse response = await new ItemNameDialog("Enter Script Name").ShowDialog(ProjectStatic.MainWindow);

            if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
            {
               // WScript script = new WScript();
              //  script.Source = @"Output.Print('Hello World!')";
             //   script.Name = response.Value;
             //   Children.Add(script);
          //      Workspace.ScriptEditor.RebuildHierarchy();
            }

        }


        public string TreeItemName { get => Name; set => Name = value; }

        public ObservableCollection<ITreeItem> Children => new ObservableCollection<ITreeItem>();



        public string Mutex;
        public string IconSource => "avares://Novovu.Workshop/Assets/COMPONENT.png";

    }
}
