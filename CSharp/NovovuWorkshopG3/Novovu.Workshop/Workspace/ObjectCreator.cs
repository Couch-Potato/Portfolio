using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Controls.ContextMenu;
using Novovu.Workshop.Converters;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Views;
using Novovu.Workshop.Views.Properties;
using Novovu.Workshop.Workspace.Toolsets;
using Novovu.Xenon.Editor;
using Novovu.Xenon.Engine;
using Novovu.Xenon.Renderer.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
    public class ObjectCreatorTaskHandler
    {
        private ObjectCreator LinkedHandler;
        public bool IsTaskExecuting = false;
        public ObjectCreatorTaskHandler(ObjectCreator creator)
        {
            LinkedHandler = creator;
        }
        public async Task ImportObjectProjectModelAsync()
        {
            IsTaskExecuting = true;
            var file = await ProjectStatic.OpenFile(ProjectStatic.ModelImportFilter);
            float modifier = 1 / (float)file.Length;
           
            float currentProgress = 0f;
            LoadDialog loadDialog = new LoadDialog("Importing assets");
            loadDialog.ShowDialog(ProjectStatic.MainWindow);
            loadDialog.SetStatus("Loading...", 0);
            foreach (var sx in file)
            {
                currentProgress += (modifier * 25f);
                
                var selected = new FileInfo(sx);
                loadDialog.SetStatus("Importing " + selected.Name, currentProgress);
                var Asset = new Models.NovovuAsset(ProjectAssets.GetTab("Objects"))
                {
                  /*  Icon = new Bitmap(File.OpenRead("Assets/3DOBJECT.png")),*/
                    Name = selected.Name.Replace(selected.Extension, ""),

                };
                Xenon.Assets.Asset asset =  await AssetStoreInterface.CompileAssetItemAsync(sx, Xenon.Assets.ParameterSets.Model);


                Asset.Selected += () =>
                {
                    WBasicModel model = new WBasicModel();
                    model.ModelHash = asset.Hash;
                    Debug.WriteLine(asset.Hash);
                    model.ParentObject = LinkedHandler.SelectedObject;
                    model.GameModel = AssetStoreInterface.AssetLoader.Load<Model>(asset.Hash);
                    LinkedHandler.SelectedObject.Models.Add(model);
                    model.Name = selected.Name.Replace(selected.Extension, "");
                    ObjectCreator.Hierarchy.GameObjects.Add(model);
                };
                ProjectAssets.AddAssetToTab("Models", Asset);
                currentProgress += (modifier * 100);
                loadDialog.SetStatus("Finalizing " + selected.Name, currentProgress);
                //Thread.Sleep(300);
            }
            loadDialog.Close();
            IsTaskExecuting = false;
        }
        public void ToggleGrid()
        {
            Engine.ControlPlane.Game.SelectedLevel.DrawGrid = !Engine.ControlPlane.Game.SelectedLevel.DrawGrid;
        }
        public void ToggleWireframe()
        {
            LinkedHandler.SelectedObject.DrawMode = LinkedHandler.SelectedObject.DrawMode == GameObject.DrawModes.Wireframe ? GameObject.DrawModes.Solid: GameObject.DrawModes.Wireframe;
        }
        public async void Save()
        {
            Engine.ControlPlane.MovementEnabled = false;
            if (LinkedHandler.Saved)
            {
                int objectcat = ProjectAssets.GetIndexOfTab("Objects");
                int itemcat = ProjectAssets.GetIndexOfAssetId(objectcat, LinkedHandler.CurrentObjectName);
                ProjectAssets.AssetTabs[objectcat].Items[itemcat].Attachment = LinkedHandler.SelectedObject;
                LinkedHandler.SelectedObject.Name = LinkedHandler.CurrentObjectName;
                ProjectAssets.SaveObjectModel(LinkedHandler.CurrentObjectName, LinkedHandler.SelectedObject);
            }else
            {
                LoadDialogResponse response = await new ItemNameDialog("Enter Object Name").ShowDialog(ProjectStatic.MainWindow);

                if (response.DialogResult == LoadDialogResponse.Result.Confirmed)
                {
                    var tab = ProjectAssets.GetTab("Objects");
                    ProjectAssets.AddAssetToTab("Objects", new Controls.AssetViewer.AssetItem(tab)
                    {
                        Name = response.Value,
                        //Icon = new Bitmap(File.OpenRead("Assets/GAMEOBJECT.png")),
                        Attachment = LinkedHandler.SelectedObject
                    });
                    LinkedHandler.CurrentObjectName = response.Value;
                    LinkedHandler.SelectedObject.Name = LinkedHandler.CurrentObjectName;
                    ObjectCreator.Hierarchy.HierarchyTitle = response.Value;
                    LinkedHandler.Saved = true;
                    ProjectModel.DiscordRPC.SetPresence("Building " + LinkedHandler.CurrentObjectName);
                }
                
            }
            Engine.ControlPlane.MovementEnabled = true;
            //Debug.WriteLine("Saved!!");

        }
        public void Clear()
        {
            LinkedHandler.CurrentObjectName = "Unnamed Object";
            LinkedHandler.Saved = false;
            ObjectCreator.Hierarchy.HierarchyTitle = "Unnamed Object";
            ProjectModel.DiscordRPC.SetPresence("Building " + LinkedHandler.CurrentObjectName);
            LinkedHandler.SelectedLevel = new Xenon.Engine.Level()
            {
                DrawGrid = true
            };
            LinkedHandler.SelectedLevel.Camera.ViewLocation = new Microsoft.Xna.Framework.Vector3(30, 30, 0);
            LinkedHandler.SelectedObject = new WGameObject();
            LinkedHandler.SelectedLevel.Objects.Add(LinkedHandler.SelectedObject);
            LinkedHandler.SelectedObject.LevelParent = LinkedHandler.SelectedLevel;
            Engine.ControlPlane.Game.SelectedLevel = LinkedHandler.SelectedLevel;
            if (Engine.ControlPlane.IsGraphicsReady)
            {
                LinkedHandler.InitEditor();
            }
            else
            {
                Engine.ControlPlane.OnGraphicsInstanceReady += (object e, EventArgs x) =>
                {
                    LinkedHandler.InitEditor();
                };
            }
            ObjectCreator.Hierarchy.GameObjects.Clear();
        }

    }
    public class ObjectCreator : Workspace
    {
        static WorkspaceInterface WorkspaceInterface = new WorkspaceInterface();
        static PropertiesViewModel Properties = new PropertiesViewModel(new Blank());
        public static NewAssetViewerViewModel AssetViewer = new NewAssetViewerViewModel();
        public static HierarchyViewModel Hierarchy = new HierarchyViewModel();
        public Level SelectedLevel;
        public WGameObject SelectedObject;
        private ObjectCreatorTaskHandler Handler;
        public string CurrentObjectName = "Unnamed Object";
        public bool Saved = false;
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
            /* ProjectModel.DiscordRPC.SetPresence("Designing Level " + Hierarchy.HierarchyTitle);
             base.Activate();*/
            ContextMenuItem itm = new ContextMenuItem();
            itm.Name = "Add Light";
            itm.OnSelect += Itm_OnSelect;
            Engine.ControlPlane.ContextMenuItems = new List<ContextMenuItem> { itm };
            ProjectModel.DiscordRPC.SetPresence("Building " + CurrentObjectName);
            base.Activate();
        }

        private void Itm_OnSelect()
        {
            SelectedObject.Lights.Add(new Xenon.Lighting.PointLight(
                    SelectedLevel.Camera.ViewLocation,
                    10f,
                    Color.Coral,
                    10f,
                    true,
                    false,
                    100,
                    100,
                    true
                ));
        }

        public ObjectCreator() : 
            base(
                "Object Creator",
                "avares://Novovu.Workshop/Assets/object-creator.png",
                Hierarchy,
                Properties,
                AssetViewer,
                Engine.EngineControl
                )
        {
            AssetStoreInterface.InitAssetLoader(Engine.ControlPlane.Services);
            ProjectStatic.ObjectRemoveInvoke += ProjectStatic_ObjectRemoveInvoke;
            ProjectStatic.ObjectDuplicateInvoke += ProjectStatic_ObjectDuplicateInvoke;
            ProjectStatic.ObjectClipboardCopyInvoke += ProjectStatic_ObjectClipboardCopyInvoke;
            ProjectStatic.ObjectClipboardPasteInvoke += ProjectStatic_ObjectClipboardPasteInvoke;
            //ProjectModel.DiscordRPC.SetPresence("Building " + CurrentObjectName);
            
            Handler = new ObjectCreatorTaskHandler(this);
            Engine.EngineControl.WorkspaceInterface = WorkspaceInterface;
            ContextMenuItem itm = new ContextMenuItem();
            itm.Name = "Add Light";
            itm.OnSelect += Itm_OnSelect;
            Engine.ControlPlane.ContextMenuItems = new List<ContextMenuItem> { itm };
            SelectedLevel = new Xenon.Engine.Level()
            {
                DrawGrid = true
            };
            SelectedLevel.Camera.ViewLocation = new Microsoft.Xna.Framework.Vector3(30, 30, 0);


            Hierarchy.HierarchyTitle = "Unnamed Object";
            

            Engine.ControlPlane.EngineKeyDown += ControlPlane_EngineKeyDown;
            Engine.ControlPlane.EngineKeyUp += ControlPlane_EngineKeyUp;

            SelectedObject = new WGameObject();
            SelectedLevel.Objects.Add(SelectedObject);
            SelectedObject.LevelParent = SelectedLevel;

            Hierarchy.SelectionUpdated += (object[] items) =>
            {
                if (items.Length > 0)
                {
                    Properties.DisplayProperty(items[0]);
                    var x = items[0];
                    
                }
                else
                {
                    Properties.ClearView();
                }
            };
            //Only here for init time handling
            Engine.ControlPlane.Game.SelectedLevel = SelectedLevel;
            if (Engine.ControlPlane.IsGraphicsReady)
            {
                InitEditor();
                //SelectedLevel.InitRenderer();
            }
            else
            {
                Engine.ControlPlane.OnGraphicsInstanceReady += (object e, EventArgs x) =>
                {
                    InitEditor();
                    //SelectedLevel.InitRenderer();
                };
            }

            ProjectAssets.GetTab("Components").ItemInsertRequested += (Controls.AssetViewer.AssetItem asset) =>
            {
                
                if (Active)
                {
                    if (asset.Attachment is WComponent)
                    {
                        Debug.WriteLine(asset.Attachment.GetType());
                        var comp = (WComponent)asset.Attachment;
                        SelectedObject.Components.Add(comp);
                        
                        //Hierarchy.HierarchyTitle = asset.ItemName;
                        //comp.DisableContextMenu();

                        Hierarchy.GameObjects.Add(comp);
                    }
                }
            };
            ProjectAssets.GetTab("Materials").ItemInsertRequested += (Controls.AssetViewer.AssetItem material) =>
            {
                if (Active)
                {
                    if (material.Attachment is MaterialEffect)
                    {
                        var mat = (MaterialEffect)material.Attachment;

                        if (selectionPool[0] is BasicModel)
                        {
                            ((BasicModel)selectionPool[0]).Material = mat;
                        }
                    }
                }
            };
            ProjectAssets.GetTab("Objects").ItemInsertRequested += (Controls.AssetViewer.AssetItem asset) =>
            {

                if (Active)
                {
                    if (asset.Attachment is WGameObject)
                    {

                        var comp = (WGameObject)asset.Attachment;
                        SelectedObject = comp;
                        CurrentObjectName = asset.Name;
                        SelectedLevel.Objects.Clear();
                        SelectedLevel.Objects.Add(SelectedObject);
                        ProjectModel.DiscordRPC.SetPresence("Building " + asset.Name);
                        Hierarchy.HierarchyTitle = asset.Name;
                        Saved = true;
                        Hierarchy.GameObjects.Clear();
                        foreach (ITreeItem children in comp.Children)
                        {
                            Hierarchy.GameObjects.Add(children);
                        }
                        
                    }
                }
            };
          
        }

        private void ProjectStatic_ObjectClipboardPasteInvoke(object o)
        {
            if (Clipboard is WBasicModel && Active)
            {
                var x = ((WBasicModel)Clipboard).Clone();
                Hierarchy.GameObjects.Add(x);
                Properties.DisplayProperty(x);
                SelectedObject.Models.Add(x);
                EditorControls.SetSelection(new ITransformable[] { x });
            }
        }

        private void ProjectStatic_ObjectClipboardCopyInvoke(object o)
        {
            if (o is WBasicModel && Active)
            {
                Clipboard = ((WBasicModel)o).Clone();
            }
        }

        private object Clipboard;

        private void ProjectStatic_ObjectDuplicateInvoke(object o)
        {
            if (o is WBasicModel && Active)
            {
                Hierarchy.GameObjects.Add((WBasicModel)o);
            }
        }

        private void ProjectStatic_ObjectRemoveInvoke(object o)
        {
            if (o is WBasicModel && Active)
            {
                Hierarchy.GameObjects.Remove((WBasicModel)o);
                Properties.DisplayProperty(new Blank());
                EditorControls.SetSelection(new ITransformable[] { });
            }
        }

        private bool IsControlDown = false;
        
        private void ControlPlane_EngineKeyUp(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.LeftControl && Active)
                IsControlDown = false;
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.G && Active)
                Handler?.ToggleGrid();
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.W && Active) {
                Handler?.ToggleWireframe();
            }
            if (key == Microsoft.Xna.Framework.Input.Keys.D && Active)
                Engine.ControlPlane.MovementEnabled = true;
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.A && Active)
            {
                //AnimationWorkspace Animator = new AnimationWorkspace(SelectedObject, SelectedLevel.Camera);
               // Animator.Activate();
            }
        }
        Xenon.Editor.Editor EditorControls;
        internal void InitEditor()
        {
            EditorControls = new Xenon.Editor.Editor(Engine.ControlPlane.Game.SelectedLevel);
            EditorControls.Update(null);
            Engine.ControlPlane.Game.SelectedLevel.DrawableObjects.Add(EditorControls);
            var PointerTool = new Rotate();
            EditorControls.SelectionPoolSelections = Editor.SelectionPoolType.BasicModelsOnly;
            Xenon.Engine.Engine.EventModel.EditorPropertyChanged += () =>
            {
                if (Active)
                    ProjectStatic.Recall();
            };
            PointerTool.SelectionGained = () =>
            {
                if (Active)
                    EditorControls.SetMode(Xenon.Editor.GizmoMode.Rotate);
            };
            EditorControls.Gizmo.SelectionUpdate += () =>
            {
                Properties.DisplayProperty(Properties.SelectedObject);
            };
            var ScaleTool = new Scale();
            ScaleTool.SelectionGained = () =>
            {
                if (Active)
                    EditorControls.SetMode(Xenon.Editor.GizmoMode.UniformScale);
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
                {
                    //Avalonia.Threading.Dispatcher.UIThread.Post(new Action(HandleDeSelectSafe));
                    return;
                }
                if (ent.Length == 0)
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(new Action(HandleDeSelectSafe));
                }
                   
                //Debug.WriteLine(ent.Length);
                selectionPool = ent;
                pool2 = ent;
                
                Avalonia.Threading.Dispatcher.UIThread.Post(new Action(HandleSelectSafe));
                
            };
            Hierarchy.SelectionUpdated += (object[] selected) =>
            {
                
                EditorControls.SetSelection(TranslateObjects(selected));
            };
            //EditorControls.
        }
        ITransformable[] selectionPool;
        object[] pool2;





        //bool IsSelectionRunning = false;
        private void HandleSelectSafe()
        {
            var ent = selectionPool;
            Hierarchy.SelectItems(TranslateTree(ent)[0]);
            if (ent.Length > 0 && Active)
            {
                Properties.DisplayProperty(ent[0]);
                var x = ent[0];
                
            }else
            {
                //Properties.DisplayProperty(new Blank());
            }
        }
        private void HandleDeSelectSafe()
        {
            Hierarchy.SelectItems(new object[0]);
            //Properties.DisplayProperty(new Blank());
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
                IsControlDown= true;
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.I && !Handler.IsTaskExecuting && Active)
            {
                //Do import task here.
                Handler?.ImportObjectProjectModelAsync();
                IsControlDown = false;
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.S && Active)
            {
                if (!Active)
                    return;
                
                Handler?.Save();
                
                IsControlDown = false;
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.N && Active)
            {

                Handler?.Clear();
            }
            if (!IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.Back)
            {
                if (!Active)
                    return;

                var obj = ((WBasicModel)EditorControls.Selected);
                obj.ParentObject.Models.Remove(obj);
                ProjectStatic_ObjectRemoveInvoke(EditorControls.Selected);
                
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
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.R)
            {
                if (!Active)
                    return;
                ProjectStatic.RenderLevel(SelectedLevel);
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.D)
            {
                if (!Active)
                    return;
                Engine.ControlPlane.MovementEnabled = false;
                try
                {
                    var obj = ((WBasicModel)EditorControls.Selected).Clone();
                    obj.Mutex = Utility.RandomString(16);
                    obj.ParentObject.Models.Add(obj);
                    ProjectStatic_ObjectDuplicateInvoke(obj);
                }
                catch { }
                
                IsControlDown = false;
            }
            if (IsControlDown && key == Microsoft.Xna.Framework.Input.Keys.Back && !Handler.IsTaskExecuting && Active)
            {

                Handler.Clear();
                IsControlDown = false;
            }
            

        }
    }
}
