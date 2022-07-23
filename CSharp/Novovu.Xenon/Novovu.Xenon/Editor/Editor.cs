using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.Editor
{
    public class Editor : Engine.IMiscDrawable
    {
        Level Level;
        GizmoComponent Gizmo;
        public Editor(Level CurrentLevel)
        {
            GizmoComponent component = new GizmoComponent(Engine.Engine.graphicsDevice);
            Gizmo = component;
            Level = CurrentLevel;
            Gizmo.TranslateEvent += Gizmo_TranslateEvent;
            Gizmo.RotateEvent += Gizmo_RotateEvent;
            Gizmo.ScaleEvent += Gizmo_ScaleEvent;
        }

        private void Gizmo_ScaleEvent(ITransformable transformable, TransformationEventArgs e)
        {
            Vector3 delta = (Vector3)e.Value;
            if (Gizmo.ActiveMode == GizmoMode.UniformScale)
                transformable.Scale *= 1 + ((delta.X + delta.Y + delta.Z) / 3);
            else
                transformable.Scale += delta;
            transformable.Scale = Vector3.Clamp(transformable.Scale, Vector3.Zero, transformable.Scale);
            Engine.Engine.EventModel.InvokeChange(EventModel.ChangeType.Scale, transformable.Scale);
        }

        private void Gizmo_RotateEvent(ITransformable transformable, TransformationEventArgs e)
        {
            Gizmo.RotationHelper(transformable, e);
            Engine.Engine.EventModel.InvokeChange(EventModel.ChangeType.Rotate, new Vector3(0,0,0));
        }

        private void Gizmo_TranslateEvent(ITransformable transformable, TransformationEventArgs e)
        {
            transformable.Position += (Vector3)e.Value;
            Engine.Engine.EventModel.InvokeChange(EventModel.ChangeType.Translate, transformable.Position);
        }

        private List<ITransformable> GetObjectTransformablesFromLevel(List<ITransformable> ins, Level l)
        {
            if (l.Objects[0] != null)
            {
                GameObject HostObject = l.Objects[0];
                ins = GetModelTransformables(ins, HostObject);
                ins = GetObjectTransformablesFromObject(ins, HostObject, true);
            }
            return ins;
        }
        private List<ITransformable> GetObjectTransformablesFromObject(List<ITransformable> ins, GameObject trans, bool ignoreFirst = false)
        {
            if (!ignoreFirst)
            {
                ins.Add(trans);
            }
            
            foreach (GameObject child in trans.Children)
            {
                ins = GetObjectTransformablesFromObject(ins, child);
            }

            return ins;
        }
        private List<ITransformable> GetModelTransformables(List<ITransformable> ins, GameObject trans)
        {
            foreach (BasicModel model in trans.Models)
            {
                ins.Add(model);
            }
            return ins;
        }
        private KeyboardState _previousKeys;
        private MouseState _previousMouse;
        private MouseState _currentMouse;
        private KeyboardState _currentKeys;
        private bool IsNewButtonPress(Keys key)
        {
            return _currentKeys.IsKeyDown(key) && _previousKeys.IsKeyUp(key);
        }
        public void Update(GameTime t)
        {
            Gizmo.SetSelectionPool(GetObjectTransformablesFromLevel(new List<ITransformable>(), Level));
            _currentMouse = Mouse.GetState();
            _currentKeys = Keyboard.GetState();
            Gizmo.UpdateCameraProperties(Level.Camera.ViewMatrix, Level.Camera.ProjectionMatrix, Level.Camera.ViewLocation);

            if (_currentMouse.LeftButton == ButtonState.Pressed && _previousMouse.LeftButton == ButtonState.Released)
                Gizmo.SelectEntities(new Vector2(_currentMouse.X, _currentMouse.Y),
                                      _currentKeys.IsKeyDown(Keys.LeftControl) || _currentKeys.IsKeyDown(Keys.RightControl),
                                      _currentKeys.IsKeyDown(Keys.LeftAlt) || _currentKeys.IsKeyDown(Keys.RightAlt));

            if (_currentKeys.IsKeyDown(Keys.LeftShift) || _currentKeys.IsKeyDown(Keys.RightShift))
                Gizmo.PrecisionModeEnabled = true;
            else
                Gizmo.PrecisionModeEnabled = false;

            // toggle active space
            if (IsNewButtonPress(Keys.O))
                Gizmo.ToggleActiveSpace();

            // toggle snapping
            if (IsNewButtonPress(Keys.I))
                Gizmo.SnapEnabled = !Gizmo.SnapEnabled;

            // select pivot types
            if (IsNewButtonPress(Keys.P))
                Gizmo.NextPivotType();

            // clear selection
            if (IsNewButtonPress(Keys.Escape))
                Gizmo.Clear();

            if (IsNewButtonPress(Keys.E))
            {
                Gizmo.ActiveMode = GizmoMode.Translate;
            }

            if (IsNewButtonPress(Keys.R))
            {
                Gizmo.ActiveMode = GizmoMode.Rotate;
            }
            if (IsNewButtonPress(Keys.T))
            {
                Gizmo.ActiveMode = GizmoMode.NonUniformScale;
            }
            if (IsNewButtonPress(Keys.Y))
            {
                Gizmo.ActiveMode = GizmoMode.UniformScale;
            }
            _previousKeys = _currentKeys;
            _previousMouse = _currentMouse;

            Gizmo.Update(t);
        }
        public void Draw(Camera c)
        {
            Gizmo.Draw();
        }
    }
}
