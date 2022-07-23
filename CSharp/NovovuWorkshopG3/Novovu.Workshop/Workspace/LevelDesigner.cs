using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Workshop.Converters;
using Novovu.Workshop.Models;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Workshop.Workspace.Toolsets;
using Novovu.Xenon.Assets;
using Novovu.Xenon.Editor;
using Novovu.Xenon.Engine;
using Novovu.Xenon.ScriptEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
    public class LevelDesignerTaskHandler
    {
        private LevelDesigner LinkedHandler;
        public bool IsTaskExecuting = false;
        public LevelDesignerTaskHandler(LevelDesigner creator)
        {
            LinkedHandler = creator;
        }

        public async void ImportObject()
        {
            var file = await ProjectStatic.OpenFile(ProjectStatic.NovovuObjectFilter);
            if (file.Length > 0)
            {
                WGameObject opened = ProjectStatic.ImportObject(file[0], LinkedHandler.SelectedLevel);
                ProjectAssets.AddAssetToTab("Objects", new Controls.AssetViewer.AssetItem(ProjectAssets.GetTab("Objects"))
                {
/*                    ItemIcon= "avares://Novovu.Workshop/Assets/GAMEOBJECT.png",*/
                    Name=opened.Name,
                    Attachment=opened
                });
            }
        }

        public void ToggleGrid()
        {
            Engine.ControlPlane.Game.SelectedLevel.DrawGrid = !Engine.ControlPlane.Game.SelectedLevel.DrawGrid;
        }
        
        internal void RunGame()
        {
            ProjectStatic.RunLevel(LinkedHandler.SelectedLevel);
        }

    }
    public class LevelDesigner : Workspace
    {
        static WorkspaceInterface WorkspaceInterface = new WorkspaceInterface();
        static PropertiesViewModel Properties = new PropertiesViewModel(new Blank());
        public static NewAssetViewerViewModel AssetViewer = new NewAssetViewerViewModel();
        public static HierarchyViewModel Hierarchy = new HierarchyViewModel();
       // static EngineControlViewModel EngineControlViewModel = new EngineControlViewModel();
        public Xenon.Engine.Level SelectedLevel;
        private LevelDesignerTaskHandler Handler;

        public new void Activate()
        {
            Engine.GenerateControlPlane();
            AssetViewer = new NewAssetViewerViewModel();
            bottomHost = AssetViewer;
            AssetStoreInterface.InitAssetLoader(Engine.ControlPlane.Services);
            Engine.ControlPlane.Game.SelectedLevel = SelectedLevel;
            Engine.ControlPlane.InvalidateVisual();
            Engine.ControlPlane.RefreshGLWindow();
            Engine.ControlPlane.DrawConsole = false;
            if (Engine.ControlPlane.IsGraphicsReady)
            {
                InitEditor();
            }
            else
            {
                Engine.ControlPlane.OnGraphicsInstanceReady += (object e, EventArgs x) =>
                {
                    InitEditor();
                };
            }
            ProjectModel.DiscordRPC.SetPresence("Designing Level " + Hierarchy.HierarchyTitle);
            base.Activate();
        }
        internal void RunGame()
        {
            Handler?.RunGame();
        }
        public LevelDesigner() :
            base(
                "Level Designer",
                "avares://Novovu.Workshop/Assets/LEVELDESIGNER.png",
               Hierarchy,
                Properties,
                AssetViewer,
                Engine.EngineControl
                )
        {
            Handler = new LevelDesignerTaskHandler(this);
            SelectedLevel = new Xenon.Engine.Level()
            {
                DrawGrid = true
            };
            foreach (var repo in (AssetRepository<ComponentRepository>)SelectedLevel.Repository.Get<ComponentRepository>())
            {
                Debug.WriteLine(repo.Key);
                ProjectAssets.ComponentRepositories.Add(repo.Key, repo.Value);
                ProjectAssets.GetTab("Components").Items.Add(new Controls.AssetViewer.ComponenetRepoAsset(ProjectAssets.GetTab("Components"), WComponentRepo.FromExistingRepo(repo.Value)));
            }
            SelectedLevel.Repository.Set<ComponentRepository>(ProjectAssets.ComponentRepositories);
            ProjectStatic.ObjectRemoveInvoke += ProjectStatic_ObjectRemoveInvoke;
            ProjectStatic.ObjectDuplicateInvoke += ProjectStatic_ObjectDuplicateInvoke;
            ProjectStatic.ObjectClipboardCopyInvoke += ProjectStatic_ObjectClipboardCopyInvoke;
            ProjectStatic.ObjectClipboardPasteInvoke += ProjectStatic_ObjectClipboardPasteInvoke;
            SelectedLevel.Camera.ViewLocation = new Vector3(30, 30, 0);
            Hierarchy.HierarchyTitle = "Unnamed Level";
            Engine.ControlPlane.EngineKeyDown += ControlPlane_EngineKeyDown;
            Engine.ControlPlane.EngineKeyUp += ControlPlane_EngineKeyUp;
            Hierarchy.SelectionUpdated += (object[] items) =>
            {
                if (items.Length > 0)
                {
                    Properties.DisplayProperty(items[0]);
                }
                else
                {
                    Properties.ClearView();
                }
            };
            ProjectAssets.GetTab("Objects").ItemInsertRequested += (Controls.AssetViewer.AssetItem item) =>
            {
                if (Active)
                {
                    var formatter = new BinaryFormatter();
                    var NewGameObject = (WGameObject)item.Attachment;
                    NewGameObject = NewGameObject.Clone();
                    NewGameObject.LevelParent = SelectedLevel;
                    SelectedLevel.Objects.Add(NewGameObject);
                    SelectedLevel.CollissionDetectors.AddDetector(NewGameObject.BoundingBox, NewGameObject);
                    NewGameObject.Name = item.Name;
                    LevelDesigner.Hierarchy.GameObjects.Add(NewGameObject);
                }
            };
        }
        private bool IsControlDown = false;

        private void ProjectStatic_ObjectClipboardPasteInvoke(object o)
        {
            if (Clipboard is WGameObject && Active)
            {
                var x = ((WGameObject)Clipboard).Clone();
                Hierarchy.GameObjects.Add(x);
                Properties.DisplayProperty(x);
                SelectedLevel.Objects.Add(x);
                EditorControls.SetSelection(new ITransformable[] { x });
            }
        }

        private void ProjectStatic_ObjectClipboardCopyInvoke(object o)
        {
            if (o is WGameObject && Active)
            {
                Clipboard = ((WGameObject)o).Clone();
            }
        }

        private object Clipboard;

        private void ProjectStatic_ObjectDuplicateInvoke(object o)
        {
            if (o is WGameObject && Active)
            {
                Hierarchy.GameObjects.Add((WGameObject)o);
            }
        }

        private void ProjectStatic_ObjectRemoveInvoke(object o)
        {
            if (o is WGameObject && Active)
            {
                Hierarchy.GameObjects.Remove((WGameObject)o);
                Properties.DisplayProperty(new Blank());
                EditorControls.SetSelection(new ITransformable[] { });
            }
        }

        private void ControlPlane_EngineKeyUp(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.LeftControl)
                IsControlDown = false;
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.G)
                Handler?.ToggleGrid();
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.W)
            {
                //Handler?.ToggleWireframe();
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.R)
                Handler?.RunGame();

        }
        Editor EditorControls;
        private void InitEditor()
        {
            EditorControls = new Xenon.Editor.Editor(Engine.ControlPlane.Game.SelectedLevel);
            EditorControls.Update(null);
            Engine.ControlPlane.Game.SelectedLevel.DrawableObjects.Add(EditorControls);
            EditorControls.SelectionPoolSelections = Editor.SelectionPoolType.GameObjectsOnly;
            var PointerTool = new Rotate();
            PointerTool.SelectionGained = () =>
            {
                if (Active)
                    EditorControls.SetMode(Xenon.Editor.GizmoMode.Rotate);
            };
            var ScaleTool = new Scale();
            ScaleTool.SelectionGained = () =>
            {
                if (Active)
                    EditorControls.SetMode(Xenon.Editor.GizmoMode.Translate);
            };
            var MoveTool = new Move();
            MoveTool.SelectionGained = () =>
            {
                if (Active)
                    EditorControls.SetMode(Xenon.Editor.GizmoMode.Translate);
            };
            Engine.ControlPlane.Tools = new List<Models.ToolItem>()
            {
                MoveTool,
                PointerTool,
                ScaleTool,
            };
            Engine.ControlPlane.QuickButtons = new List<Models.ToolItem>();
            Engine.ControlPlane.SelectedTool = Engine.ControlPlane.Tools[0];
            EditorControls.SetMode(Xenon.Editor.GizmoMode.Translate);
            Engine.ControlPlane.Game.SelectedLevel.Tasks.Add((GameTime t) =>
            {
                EditorControls.Update(t);
                return false;
            });

            EditorControls.OnItemSelected += (ITransformable[] ent) =>
            {
                if (selectionPool != null && selectionPool[0] == ent[0])
                    return;
                //Debug.WriteLine(ent.Length);
                selectionPool = ent;
                pool2 = ent;

                Avalonia.Threading.Dispatcher.UIThread.Post(new Action(HandleSelectSafe));

            };
            Hierarchy.SelectionUpdated += (object[] selected) =>
            {
                if (Active)
                    EditorControls.SetSelection(TranslateObjects(selected));
            };
        }
        ITransformable[] selectionPool;
        object[] pool2;
        //bool IsSelectionRunning = false;
        private void HandleSelectSafe()
        {
            var ent = selectionPool;
            //Hierarchy.SelectItems(TranslateTree(ent));
            if (ent.Length > 0)
            {
                Properties.DisplayProperty(ent[0]);
            }
        }
        private ITransformable[] TranslateObjects(object[] selected)
        {
            List<ITransformable> transformables = new List<ITransformable>();
            foreach (object o in selected)
            {
                if (o is ITransformable)
                    transformables.Add((ITransformable)o);
            }
            return transformables.ToArray();
        }
        private ITreeItem[] TranslateTree(object[] tree)
        {
            List<ITreeItem> treeItems = new List<ITreeItem>();
            foreach (object o in tree)
                if (o is ITreeItem)
                    treeItems.Add((ITreeItem)o);
            return treeItems.ToArray();
        }
        private void ControlPlane_EngineKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.LeftControl)
                IsControlDown = true;
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.I && Active)
            {
                IsControlDown = false;
                Handler?.ImportObject();
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.C)
            {
                if (!Active)
                    return;
                ProjectStatic_ObjectClipboardCopyInvoke(EditorControls.Selected);
                IsControlDown = false;
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.V)
            {
                if (!Active)
                    return;
                ProjectStatic_ObjectClipboardPasteInvoke(EditorControls.Selected);
                IsControlDown = false;
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.D)
            {
                if (!Active)
                    return;
                try
                {
                    if (EditorControls.Selected != null){
                        var obj = ((WGameObject)EditorControls.Selected).Clone();
                        obj.Mutex = Utility.RandomString(16);
                        obj.LevelParent.Objects.Add(obj);
                        ProjectStatic_ObjectDuplicateInvoke(obj);
                    }
                    
                }
                catch { }

                IsControlDown = false;
            }
        }
    }
}
