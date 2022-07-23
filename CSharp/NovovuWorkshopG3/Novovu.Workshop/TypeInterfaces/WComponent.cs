
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.TypeInterfaces
{
    public class WComponent : Component, ITreeItem, IContextable, Novovu.Workshop.Models.IIconable, IWComponent
    {
        public ContextOption[] ContextOptions { get; private set; }

        public delegate void ScriptContentsUpdated();
        public event ScriptContentsUpdated ComponentUpdated;

        public string _ent = "main.js";
        public string _prop = "properties.js";
        
        [Property("Entry Script", "Basic", Property.PropertyType.String)]
        public string Entry { get => _ent; set => _ent = value; }
        [Property("Properties", "Basic", Property.PropertyType.String)]
        public string PropertiesScriptPoint { get => _prop; set => _prop = value; }

        public WComponentRepo Parent;

       
        public WComponent(string componentName,  bool isReadOnly,WComponentRepo parent=null)
        {
            Children = new ObservableCollection<ITreeItem>();
            if (!isReadOnly)
            {
                var AddScript = new ContextOption("Add Script");
                AddScript.ContextClick += AddScript_ContextClick;
                var Delete = new ContextOption("Delete Component");
                Delete.ContextClick += Delete_ContextClick;
                var Rename = new ContextOption("Rename Component");
                Rename.ContextClick += Rename_ContextClick;
                ContextOptions = new ContextOption[]
                {
                    AddScript,
                    Delete, Rename
                };
                var script = new WScript(Name, isReadOnly, this);
                script.Name = "main";
                script.Source = "Output.Print('Hello World!')";
                this.Scripts = new Xenon.ScriptEngine.Script[] { script };
                this.EntryPointScript = script;
                //this.PropertiesScript = script;
                Children.Add(script);
            }
            Name = componentName;
            Parent = parent;
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

        private async void AddScript_ContextClick()
        {
            LoadDialogResponse response = await new ItemNameDialog("Enter Script Name").ShowDialog(ProjectStatic.MainWindow);

            if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
            {
                WScript script = new WScript(response.Value, false, this);
                script.Source = @"Output.Print('Hello World!')";
                script.Name = response.Value;
                Children.Add(script);
            }
            
        }

        public string Dir { get; set; }
        public string TreeItemName { get => Name; set => Name = value; }

        public ObservableCollection<ITreeItem> Children { get; }

        public WComponent()
        {
            Mutex = Utility.RandomString(16);
        }

        public string Mutex;
        public string IconSource => "avares://Novovu.Workshop/Assets/COMPONENT.png";
    }
}
