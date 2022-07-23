using Microsoft.Xna.Framework;
using Novovu.Controls.KeyframeEditor;
using Novovu.Workshop.Converters;
using Novovu.Workshop.ProjectModel;
using Novovu.Workshop.Shared;
using Novovu.Workshop.TypeInterfaces;
using Novovu.Workshop.ViewModels;
using Novovu.Workshop.Workspace.Toolsets;
using Novovu.Xenon.Editor;
using Novovu.Xenon.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Workshop.Workspace
{
    public class AnimationWorkspace : Workspace
    {
        public static HierarchyViewModel Hierarchy = new HierarchyViewModel();
        static PropertiesViewModel Properties = new PropertiesViewModel(new Blank());
        public Level SelectedLevel;
        public WGameObject SelectedObject;
        public string CurrentObjectName = "Unnamed Object";
        private WGameObject LastState;
        public new void Activate()
        {
            Engine.GenerateControlPlane();

            Hierarchy.HierarchyTitle = SelectedObject.Name;

            Hierarchy.GameObjects.Clear();
            foreach (ITreeItem child in SelectedObject.Children)
            {
                Hierarchy.GameObjects.Add(child);
            }

            

            SelectedLevel = new Xenon.Engine.Level()
            {
                DrawGrid = true
            };
            SelectedLevel.Camera.ViewLocation = cd.ViewLocation;
            SelectedLevel.Camera.LookAtVector = cd.LookAtVector;
            
            SelectedObject.LevelParent = SelectedLevel;
            SelectedLevel.Objects.Add(SelectedObject);
            LastState = SelectedObject.Clone();

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
            Engine.ControlPlane.Game.SelectedLevel = SelectedLevel;
            Engine.ControlPlane.EngineKeyDown += ControlPlane_EngineKeyDown;
            Engine.ControlPlane.EngineKeyUp += ControlPlane_EngineKeyUp;
            ProjectModel.DiscordRPC.SetPresence("Animating " + CurrentObjectName);
            base.Activate();
        }
        private Camera cd;
        public AnimationWorkspace(WGameObject objects, Camera cdata) : base(
            "Animation Editor",
            "avares://Novovu.Workshop/Assets/object-creator.png",
             Hierarchy,
                Properties,
            new KeyFrameViewModel(),
            Engine.EngineControl
            )
        {
            SelectedObject = objects.Clone();
            cd = cdata;
            Engine.Editor.ScrubberMoved += Editor_ScrubberMoved;

        }

        private void Editor_ScrubberMoved(float position)
        {
            var pc = position / 660f;
            foreach (Layer l in Engine.Editor.Layers)
            {

            }
        }

        private void ControlPlane_EngineKeyUp(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.LeftControl)
                IsControlDown = false;
            if (key == Microsoft.Xna.Framework.Input.Keys.K && IsControlDown && Active)
            {
                // This is the most ugliest shit code ever written.. oh well
                //Compare our states and see if any models were changed
                foreach (BasicModel mdl in SelectedObject.Models)
                {
                    foreach (BasicModel bx in LastState.Models)
                    {
                        if (bx.Position != mdl.Position || bx.OrientationOffset != mdl.OrientationOffset || bx.Scale != mdl.Scale)
                        {
                            if (bx.Name == mdl.Name)
                            {
                                bool LayerExists = false;
                                // See if there is a layer that exists
                                foreach (Layer l in Engine.Editor.Layers)
                                {
                                    if (l.Name == mdl.Name)
                                    {
                                        l.Keyframes.Add(new KeyFrame()
                                        {
                                            Data = new WKeyframe()
                                            {
                                                Position = mdl.Position,
                                                Rotation = mdl.OrientationOffset,
                                                Scale = mdl.Scale
                                            },
                                            Position = Engine.Editor.Scrubber.Position
                                        });
                                        LayerExists = true;
                                    }
                                }
                                if (!LayerExists)
                                {
                                    //Create a new layer
                                    Layer newl = new Layer();
                                    //Create our initial keyframe for our last state so we can have better intermesthing
                                    newl.Keyframes.Add(new KeyFrame()
                                    {
                                        Data = new WKeyframe()
                                        {
                                            Position = bx.Position,
                                            Rotation = bx.OrientationOffset,
                                            Scale = bx.Scale
                                        }
                                    });

                                    // Create a keyframe for our latest area

                                    newl.Keyframes.Add(new KeyFrame()
                                    {
                                        Data = new WKeyframe()
                                        {
                                            Position = mdl.Position,
                                            Rotation = mdl.OrientationOffset,
                                            Scale = mdl.Scale
                                        },
                                        Position = Engine.Editor.Scrubber.Position
                                    });
                                    newl.Name = mdl.Name;
                                    Engine.Editor.Layers.Add(newl);
                                }
                            }
                        }
                    }
                }

                //Save our latest object state
                LastState = SelectedObject.Clone();
            }
        }

        private void ControlPlane_EngineKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (key == Microsoft.Xna.Framework.Input.Keys.LeftControl)
                IsControlDown = true;
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
                    return;
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
        private void HandleSelectSafe()
        {
            var ent = selectionPool;
            //Hierarchy.SelectItems(TranslateTree(ent));
            if (ent.Length > 0 && Active)
            {
                Properties.DisplayProperty(ent[0]);
                var x = ent[0];

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

        private bool IsControlDown = false;


    }
}
