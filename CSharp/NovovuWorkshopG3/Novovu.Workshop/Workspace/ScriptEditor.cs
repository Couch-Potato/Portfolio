using Avalonia.Media.TextFormatting.Unicode;
using Microsoft.Xna.Framework;
using Novovu.Workshop.Controls.AssetViewer;
using Novovu.Workshop.Converters;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Workshop.Views.Dialogs;
using Novovu.Workshop.Workspace.Toolsets;
using Novovu.Xenon.Engine;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
 
    public class ScriptEditor : Workspace
    {
        public static AssetViewerViewModel AssetViewer = new AssetViewerViewModel();
        public static HierarchyViewModel Hierarchy = new HierarchyViewModel();
        public static CodeEditorViewModel Code = new CodeEditorViewModel();
        private static WComponentRepo ProjectRoot = new WComponentRepo("project", false);
        public static List<WComponentRepo> Repositories = new List<WComponentRepo>();
        public new void Activate()
        {
            RebuildHierarchy();
            base.Activate();
        }
        public void OpenCode(string name, string source, WScript s)
        {
            CodeEditorViewModel cevm = new CodeEditorViewModel();
            cevm.Title = name;
            SetActiveDocument(cevm);
            cevm.OpenCode(source, ref s);
          
        }

        public static void RebuildHierarchy()
        {
            Hierarchy.GameObjects.Clear();
            Hierarchy.GameObjects.Add(ProjectRoot);
            foreach (WComponentRepo repo in Repositories)
            {
                Hierarchy.GameObjects.Add(repo);
            }

        }
        public ScriptEditor()
            :base(
                 "Script Editing",
                 "avares://Novovu.Workshop/Assets/SCRIPTEDITOR.png",
                 Hierarchy,
                null,
                null,
               null
                 )
        {
            
            IsMainDocument = true;
            Hierarchy.HierarchyTitle = "Game Components";
            Hierarchy.GameObjects.Clear();
            Hierarchy.GameObjects.Add(ProjectRoot);
            var cpx = new ComponenetRepoAsset(ProjectAssets.GetTab("Components"), ProjectRoot);
            cpx.Name = ProjectRoot.TreeItemName;
            
            

            ProjectAssets.AddAssetToTab("Components", cpx);
            foreach (WComponentRepo repo in Repositories)
            {
                Hierarchy.GameObjects.Add(repo);
                var cpx2 = new ComponenetRepoAsset(ProjectAssets.GetTab("Components"), repo);
                cpx2.Name = repo.TreeItemName;
                ProjectAssets.AddAssetToTab("Components", cpx2);
            }


        }

    }
}
