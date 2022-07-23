using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.Editor
{
    [Obsolete("Use Editor Class instead")]
    public class EditorScheme : Engine.IMiscDrawable
    {
        IBoundHolder boundHolder;
        IManipulatable manipulatable;
        Level Holder;
        public EditorScheme(IBoundHolder holder, Level l, IManipulatable manipulatable)
        {
            Holder = l;
            boundHolder = holder;
            this.manipulatable = manipulatable;
            SetSchemeState(EditorMode);
        }
        
        
        private const float LINE_LENGTH = 3f;
        private const float LINE_OFFSET = 1f;
        private Matrix EditorWorld = Matrix.Identity;

        Matrix[] Locals = new Matrix[3]
        {
            Matrix.CreateWorld(new Vector3(LINE_LENGTH, 0, 0), Vector3.Left, Vector3.Up),
            Matrix.CreateWorld(new Vector3(0, LINE_LENGTH, 0), Vector3.Down, Vector3.Left),
            Matrix.CreateWorld(new Vector3(0, 0, LINE_LENGTH), Vector3.Forward, Vector3.Up),
        };

        Color[] AxisColors = new Color[3]
        {
            Color.Red,
            Color.Green,
            Color.Blue
        };

        public enum Mode
        {
            Scale,
            Translate,
            Rotate,
            UniformScale
        }

        private VertexPositionColor[] TLineVert = GeometricObjects.TranslationLine;

        private BasicEffect LineEffec = new BasicEffect(Engine.Engine.graphicsDevice);
        private BasicEffect Effx = new BasicEffect(Engine.Engine.graphicsDevice);
        private BasicEffect qEffect = new BasicEffect(Engine.Engine.graphicsDevice);
        public Mode EditorMode = Mode.Translate;

        bool[] HoverStatus = new bool[3]
        {
            false,
            false,
            false
        };
        bool[] SelectionStats = new bool[3]
        {
            false,
            false,
            false
        };
        Vector3 Offset = new Vector3(0, 0, 0);
        Vector3 ScaleOffset = new Vector3(0, 0, 0);
        public void Update(GameTime g)
        {
            if (mouseOneDown)
            {
                if (EditorMode == Mode.Translate)
                {
                    if (SelectionStats[0])
                    {
                        MouseState mstate = Mouse.GetState();
                        BoundingBox selected = boundHolder.GetBoundingBox();
                        Vector3 Magnitude = selected.Max - selected.Min;
                        Vector3 Position = (Magnitude / 2) + selected.Min;
                        Plane p = new Plane(new Vector3(-Holder.Camera.farClipPlane, Position.Y, -Holder.Camera.farClipPlane), new Vector3(-Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane), new Vector3(Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane));
                        Vector3 pos = Holder.Camera.ScreenToWorldPlane(p, mstate.Position.ToVector2());
                        if (Offset == new Vector3(0, 0, 0))
                        {
                            //Debug.WriteLine(manipulatable.GetPosition());
                            Offset = pos - manipulatable.GetPosition();
                        }
                        var ap = manipulatable.GetPosition();
                        manipulatable.SetPosition(new Vector3(pos.X - Offset.X, ap.Y, ap.Z));
                    }
                    if (SelectionStats[2])
                    {
                        MouseState mstate = Mouse.GetState();
                        BoundingBox selected = boundHolder.GetBoundingBox();
                        Vector3 Magnitude = selected.Max - selected.Min;
                        Vector3 Position = (Magnitude / 2) + selected.Min;
                        Plane p = new Plane(new Vector3(-Holder.Camera.farClipPlane, Position.Y, -Holder.Camera.farClipPlane), new Vector3(-Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane), new Vector3(Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane));
                        Vector3 pos = Holder.Camera.ScreenToWorldPlane(p, mstate.Position.ToVector2());
                        if (Offset == new Vector3(0, 0, 0))
                        {
                            //Debug.WriteLine(manipulatable.GetPosition());
                            Offset = pos - manipulatable.GetPosition();
                        }

                        var ap = manipulatable.GetPosition();
                        manipulatable.SetPosition(new Vector3(ap.X, ap.Y, pos.Z - Offset.Z));
                    }
                    if (SelectionStats[1])
                    {
                        MouseState mstate = Mouse.GetState();
                        BoundingBox selected = boundHolder.GetBoundingBox();
                        Vector3 Magnitude = selected.Max - selected.Min;
                        Vector3 Position = (Magnitude / 2) + selected.Min;
                        Plane p = new Plane(new Vector3(-Holder.Camera.farClipPlane, -Holder.Camera.farClipPlane, Position.Z), new Vector3(-Holder.Camera.farClipPlane, Holder.Camera.farClipPlane, Position.Z), new Vector3(Holder.Camera.farClipPlane, Holder.Camera.farClipPlane, Position.Z));
                        Vector3 pos = Holder.Camera.ScreenToWorldPlane(p, mstate.Position.ToVector2());
                        if (Offset == new Vector3(0, 0, 0))
                        {
                            //Debug.WriteLine(manipulatable.GetPosition());
                            Offset = pos - manipulatable.GetPosition();
                        }
                        var ap = manipulatable.GetPosition();
                        manipulatable.SetPosition(new Vector3(ap.X, pos.Y - Offset.Y, ap.Z));
                    }
                }
                if (EditorMode ==  Mode.Scale)
                {
                    if (SelectionStats[0])
                    {
                        MouseState mstate = Mouse.GetState();
                        BoundingBox selected = boundHolder.GetBoundingBox();
                        Vector3 Magnitude = selected.Max - selected.Min;
                        Vector3 Position = (Magnitude / 2) + selected.Min;
                        Plane p = new Plane(new Vector3(-Holder.Camera.farClipPlane, Position.Y, -Holder.Camera.farClipPlane), new Vector3(-Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane), new Vector3(Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane));
                        Vector3 pos = Holder.Camera.ScreenToWorldPlane(p, mstate.Position.ToVector2());
                        if (Offset == new Vector3(0, 0, 0))
                        {
                            //Debug.WriteLine(manipulatable.GetPosition());
                            Offset = pos - manipulatable.GetPosition();
                        }
                        if (ScaleOffset == new Vector3(0, 0, 0))
                        {
                            ScaleOffset = selected.Max;
                        }
                        var ap = manipulatable.GetScale();
                        var newX = (pos.X - Offset.X) / ScaleOffset.X;
                        manipulatable.SetScale(new Vector3(newX, ap.Y,ap.Z));
                    }
                    if (SelectionStats[2])
                    {
                        MouseState mstate = Mouse.GetState();
                        BoundingBox selected = boundHolder.GetBoundingBox();
                        Vector3 Magnitude = selected.Max - selected.Min;
                        Vector3 Position = (Magnitude / 2) + selected.Min;
                        Plane p = new Plane(new Vector3(-Holder.Camera.farClipPlane, Position.Y, -Holder.Camera.farClipPlane), new Vector3(-Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane), new Vector3(Holder.Camera.farClipPlane, Position.Y, Holder.Camera.farClipPlane));
                        Vector3 pos = Holder.Camera.ScreenToWorldPlane(p, mstate.Position.ToVector2());

                        var ap = manipulatable.GetScale();
                        if (Offset == new Vector3(0, 0, 0))
                        {
                            //Debug.WriteLine(manipulatable.GetPosition());
                            Offset = pos - manipulatable.GetPosition();
                        }
                        if (ScaleOffset == new Vector3(0,0,0))
                        {
                            ScaleOffset = selected.Max;
                        }
                        var newZ = (pos.Z - Offset.Z) / ScaleOffset.Z;
                        manipulatable.SetScale(new Vector3(ap.X, ap.Y, newZ));
                        //manipulatable.SetPosition(new Vector3(lp.X, lp.Y, (pos.Z - Offset.Z)));
                    }
                    if (SelectionStats[1])
                    {
                        MouseState mstate = Mouse.GetState();
                        BoundingBox selected = boundHolder.GetBoundingBox();
                        Vector3 Magnitude = selected.Max - selected.Min;
                        Vector3 Position = (Magnitude / 2) + selected.Min;
                        Plane p = new Plane(new Vector3(-Holder.Camera.farClipPlane, -Holder.Camera.farClipPlane, Position.Z), new Vector3(-Holder.Camera.farClipPlane, Holder.Camera.farClipPlane, Position.Z), new Vector3(Holder.Camera.farClipPlane, Holder.Camera.farClipPlane, Position.Z));
                        Vector3 pos = Holder.Camera.ScreenToWorldPlane(p, mstate.Position.ToVector2());
                        if (Offset == new Vector3(0, 0, 0))
                        {
                            //Debug.WriteLine(manipulatable.GetPosition());
                            Offset = pos - manipulatable.GetPosition();
                        }
                        if (ScaleOffset == new Vector3(0, 0, 0))
                        {
                            ScaleOffset = selected.Max;
                        }
                        var ap = manipulatable.GetScale();
                        var newY = (pos.Y - Offset.Y) / ScaleOffset.Y;
                        manipulatable.SetScale(new Vector3(ap.X, newY, ap.Z));

                    }
                }

                
            }else
            {
                Offset = new Vector3(0,0,0);
                ScaleOffset = new Vector3(0, 0, 0);
            }
        }
        bool mouseOneDown = false;
        public void SetSchemeState(Mode m)
        {

            Level l = Holder;
            EditorMode = m;
            //XYZ Order
            for (int i = 0; i < 3; i++)
            {
                Geometry model;
                switch (EditorMode)
                {
                    case Mode.Rotate:
                        model = GeometricObjects.Rotate;
                        break;
                    case Mode.Translate:
                        model = GeometricObjects.Translate;
                        break;
                    default:
                        model = GeometricObjects.Scale;
                        break;
                }
                BoundingBox selected = boundHolder.GetBoundingBox();
                Vector3 Magnitude = selected.Max - selected.Min;
                Vector3 Position = (Magnitude / 2) + selected.Min;
                float Scale = Magnitude.Length() / 5f;
                Matrix ScreenScale = Matrix.CreateScale(new Vector3(5f));



                EditorWorld = ScreenScale * Matrix.CreateWorld(Position, Matrix.Identity.Forward, Matrix.Identity.Up);
                switch (i)
                {
                    case 2:
                        EditorWorld *= Matrix.CreateTranslation(0, 0, selected.Max.Z + manipulatable.GetScale().Z);
                        break;
                    case 1:
                        EditorWorld *= Matrix.CreateTranslation(0, selected.Max.Y + manipulatable.GetScale().Y, 0);
                        break;
                    case 0:
                        EditorWorld *= Matrix.CreateTranslation(selected.Max.X + manipulatable.GetScale().X, 0, 0);
                        break;
                }

                BoundingBox modelBox = model.GetBoundingBox(Locals[i] * EditorWorld);

                
                int thisObjectId = i;
                l.CollissionDetectors.AddDetector(modelBox, 2,(CollissionDetectorResult re) =>
                {
                    switch (re.InterceptType)
                    {
                        case (CollisionDetector.InterceptType.Mouse1Down):

                            mouseOneDown = true;
                            
                            SelectionStats[thisObjectId] = true;
                            break;
                        case (CollisionDetector.InterceptType.Mouse1Up):
                            mouseOneDown = false;
                            SelectionStats[thisObjectId] = false;
                            break;
                        case (CollisionDetector.InterceptType.MouseHoverStart):
                            HoverStatus[thisObjectId] = true;
                            break;
                        case (CollisionDetector.InterceptType.MouseHoverEnd):
                            
                            HoverStatus[thisObjectId] = false;
                            break;
                    }
                });

            }
            
        }

        public void Draw(Camera c)
        {
            //Editor.SelectionBox box = new SelectionBox(boundHolder.GetBoundingBox());
            //box.Draw(c);

            BoundingBox selected = boundHolder.GetBoundingBox();
            Vector3 Magnitude = selected.Max - selected.Min;
            Vector3 Position = (Magnitude / 2) + selected.Min;

            Vector3 vLen = c.ViewLocation - Position;
            float Scale = Magnitude.Length() / 5f;
            Matrix ScreenScale = Matrix.CreateScale(new Vector3(5f));



            EditorWorld = ScreenScale * Matrix.CreateWorld(Position, Matrix.Identity.Forward, Matrix.Identity.Up);

          //  LineEffec.World = EditorWorld;
         //   LineEffec.View = c.ViewMatrix;
         //   LineEffec.Projection = c.ProjectionMatrix;
//
         //   LineEffec.CurrentTechnique.Passes[0].Apply();

          //  Engine.Engine.graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, TLineVert, 0, TLineVert.Length / 2);


            #region Draw Translucent Quads
           // Engine.Engine.graphicsDevice.BlendState = BlendState.AlphaBlend;
           // Engine.Engine.graphicsDevice.RasterizerState = RasterizerState.CullNone;

          //  qEffect.World = EditorWorld;
          //  qEffect.View = c.ViewMatrix;
          //  qEffect.Projection = c.ProjectionMatrix;

       //     qEffect.CurrentTechnique.Passes[0].Apply();

            //Engine.Engine.graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, GeometricObjects.XYQuad.Vertices, 0, 4, GeometricObjects.XYQuad.Indexes, 0, 2);
            //Engine.Engine.graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, GeometricObjects.XYQuad.Vertices, 0, 4, GeometricObjects.ZYQuad.Indexes, 0, 2);
            //Engine.Engine.graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, GeometricObjects.XYQuad.Vertices, 0, 4, GeometricObjects.XZQuad.Indexes, 0, 2);

         //   Engine.Engine.graphicsDevice.BlendState = BlendState.Opaque;
         //   Engine.Engine.graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            #endregion

            for (int i = 0; i < 3; i++)
            {
                EditorWorld = ScreenScale * Matrix.CreateWorld(Position, Matrix.Identity.Forward, Matrix.Identity.Up);
                Geometry model;
                switch (EditorMode)
                {
                    case Mode.Rotate:
                        model = GeometricObjects.Rotate;
                        break;
                    case Mode.Translate:
                        model = GeometricObjects.Translate;
                        break;
                    default:
                        model = GeometricObjects.Scale;
                        break;
                }
                Vector3 Color;
                switch(EditorMode)
                {
                    case Mode.UniformScale:
                        Color = AxisColors[0].ToVector3();
                        break;
                    default:
                        Color = AxisColors[i].ToVector3();
                        break;
                }
                if (HoverStatus[i])
                {
                    Color = new Vector3(255, 255, 0);
                }

                switch (i)
                {
                    case 2:
                        EditorWorld *= Matrix.CreateTranslation(0, 0, selected.Max.Z + manipulatable.GetScale().Z);
                        break;
                    case 1:
                        EditorWorld *= Matrix.CreateTranslation(0, selected.Max.Y + manipulatable.GetScale().Y, 0);
                        break;
                    case 0:
                        EditorWorld *= Matrix.CreateTranslation(selected.Max.X + manipulatable.GetScale().X, 0, 0);
                        break;
                }

                Effx.World = Locals[i] * EditorWorld;
                Effx.View = c.ViewMatrix;
                Effx.Projection = c.ProjectionMatrix;

                Effx.DiffuseColor = Color;
                Effx.EmissiveColor = Color;

                Effx.CurrentTechnique.Passes[0].Apply();

                Engine.Engine.graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, 
                    model.Vertices, 0, model.Vertices.Length, 
                    model.Indices, 0, model.Indices.Length / 3);
            }
        }
    }
}
