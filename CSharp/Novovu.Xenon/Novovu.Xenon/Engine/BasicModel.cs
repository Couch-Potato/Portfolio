using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Editor;
using Novovu.Xenon.Lighting;
namespace Novovu.Xenon.Engine
{
    public class BasicModel : IBoundHolder, Editor.ITransformable
    {
        #region Properties
        public Model GameModel { get { return Rref; } set { Rref = value; ObjectMeshes = value.Meshes.ToArray(); } }

        private Vector3 Forwards = Vector3.Forward;
        private Vector3 Ups = Vector3.Up;

        string ITransformable.Name { get => ModelName; set => ModelName = value; }
        public Vector3 Position { get => LocationOffset; set => LocationOffset = value; }
        Vector3 ITransformable.Scale { get => Scale; set => Scale = value; }
        public Vector3 Forward { get => Forwards; set => Forwards = value; }
        public Vector3 Up { get => Ups; set => Ups = value; }

        public BoundingBox BoundingBox => GetBoundingBox();

        private string ModelName = "";

        private Model Rref;

        public Texture2D ModelTexture;

        public Materials.IMaterial Material;

        public List<Particles.ParticleEmitter> ParticleEmitters = new List<Particles.ParticleEmitter>();

        public Vector3 LocationOffset;

        public Vector3 OrientationOffset;

        [Obsolete("Use Scale property instead.")]
        public float ScaleOffset;

        public Vector3 Scale = new Vector3(0,0,0);

        public float SpecularPower = 100f;

        public GameObject ParentObject;

        public EmissiveLight EmissiveLight;

        public enum DrawModes
        {
            Solid,
            Wireframe,
            Inherit

        }

        public DrawModes DrawMode = DrawModes.Inherit;

        public ModelMesh[] ObjectMeshes;
        #endregion

        #region Engine Loop Methods

        public BoundingBox GetBoundingBox()
        {
            Matrix Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(OrientationOffset.X + ParentObject.Orientation.X)) *
                         Matrix.CreateRotationY(MathHelper.ToRadians(OrientationOffset.Y + ParentObject.Orientation.Y)) *
                         Matrix.CreateRotationZ(MathHelper.ToRadians(OrientationOffset.Z + ParentObject.Orientation.Z));
            Matrix worldTransform = Rotation * Matrix.CreateTranslation(ParentObject.Location + LocationOffset) * Matrix.CreateScale(ParentObject.Scale + Scale);

            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            // For each mesh of the model
            foreach (ModelMesh mesh in GameModel.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    // Vertex buffer parameters
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), worldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }

            // Create and return bounding box
            return new BoundingBox(min, max);
        }

        public void Draw(Camera camera, GameObject parent)
        {
            foreach (ModelMesh mesh in ObjectMeshes)
            {

                if (Material == null)
                {
                    
                    foreach (BasicEffect effect in mesh.Effects)
                    {


                        if (ParentObject.LevelParent.UseDefaultLighting)
                            effect.EnableDefaultLighting();

                        if (ModelTexture != null)
                        {
                            effect.TextureEnabled = true;
                            effect.Texture = ModelTexture;
                        }
                        if (EmissiveLight != null)
                        {
                            effect.EmissiveColor = EmissiveLight.Color;
                        }
                        effect.LightingEnabled = true;
                        IObjectLight[] Lights = ParentObject.LevelParent.GetLightsForObject(LocationOffset + parent.Location);
                        if (Lights[0] != null)
                        {
                            //Debug.WriteLine("!");
                            IObjectLight light = Lights[0];
                            effect.DirectionalLight0.Direction = Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                            
                            effect.DirectionalLight0.Enabled = true;
                            effect.DirectionalLight0.SpecularColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                );
                            effect.DirectionalLight0.DiffuseColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                );

                            
                        }
                        if (Lights[1] != null)
                        {
                            IObjectLight light = Lights[1];
                            effect.DirectionalLight1.Direction = Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                            effect.DirectionalLight1.Enabled = true;
                            effect.DirectionalLight1.SpecularColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                );
                            effect.DirectionalLight0.DiffuseColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                );
                        }
                        if (Lights[2] != null)
                        {
                            IObjectLight light = Lights[2];
                            effect.DirectionalLight2.Direction = Engine.GetVectorDirection(LocationOffset + parent.Location, light.PositionOffset + light.Parent.Location);
                            effect.DirectionalLight2.Enabled = true;
                            effect.DirectionalLight2.SpecularColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light) + .3), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light) + .3), 0, 1)
                                );
                            effect.DirectionalLight0.DiffuseColor = new Vector3(
                                MathHelper.Clamp((float)(light.Color.X * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Y * LightingPhysics.GetKatt(this, light)), 0, 1),
                                MathHelper.Clamp((float)(light.Color.Z * LightingPhysics.GetKatt(this, light)), 0, 1)
                                );
                        }
                        effect.AmbientLightColor = ParentObject.LevelParent.AmbientLight.Color;

                        Matrix Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(OrientationOffset.X + parent.Orientation.X)) *
                           Matrix.CreateRotationY(MathHelper.ToRadians(OrientationOffset.Y + parent.Orientation.Y)) *
                           Matrix.CreateRotationZ(MathHelper.ToRadians(OrientationOffset.Z + parent.Orientation.Z));
                        Matrix world = Rotation * Matrix.CreateTranslation(parent.Location + LocationOffset) * Matrix.CreateScale(parent.Scale + Scale);
                        effect.Alpha = parent.Alpha;
                        effect.World = world;
                        effect.Projection = camera.ProjectionMatrix;
                        effect.View = camera.ViewMatrix;
                    }
                }
                else
                {
                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        Matrix Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(OrientationOffset.X + parent.Orientation.X)) *
                            Matrix.CreateRotationY(MathHelper.ToRadians(OrientationOffset.Y + parent.Orientation.Y)) *
                            Matrix.CreateRotationZ(MathHelper.ToRadians(OrientationOffset.Z + parent.Orientation.Z));

                        Matrix world = Rotation * Matrix.CreateTranslation(parent.Location + LocationOffset) * Matrix.CreateScale(parent.Scale + Scale);
                        Matrix worldInverseTransposeMatrix = Matrix.Transpose(Matrix.Invert(mesh.ParentBone.Transform * world));
                        part.Effect = Material.GetMaterialEffect(world, ParentObject.LevelParent.Camera.ViewMatrix, ParentObject.LevelParent.Camera.ProjectionMatrix, worldInverseTransposeMatrix);

                    }
                }
                //Potential Issue: When drawing with a Material, the lighting engine may over-ride the material shader.

                if (DrawMode == DrawModes.Wireframe)
                {
                    RasterizerState originalState = Engine.graphicsDevice.RasterizerState;
                    RasterizerState rasterizerState = new RasterizerState();
                    rasterizerState.FillMode = FillMode.WireFrame;
                    mesh.Draw();
                    Engine.graphicsDevice.RasterizerState = rasterizerState;
                    Engine.graphicsDevice.RasterizerState = originalState;
                    
                    return;
                }
                else if (DrawMode == DrawModes.Inherit && parent.DrawMode == GameObject.DrawModes.Wireframe)
                {
                    RasterizerState originalState = Engine.graphicsDevice.RasterizerState;
                    RasterizerState rasterizerState = new RasterizerState();
                    rasterizerState.FillMode = FillMode.WireFrame;
                    mesh.Draw();
                    Engine.graphicsDevice.RasterizerState = rasterizerState;
                    Engine.graphicsDevice.RasterizerState = originalState;
                    
                    return;
                }
                mesh.Draw();


            }
        }
        public void Update(GameTime time)
        {
            foreach (Particles.ParticleEmitter particle in ParticleEmitters)
            {
                particle.Update(time, LocationOffset + particle.LocationOffset + ParentObject.Location);
            }

        }

        public void DoParticles(GameObject parent)
        {
            foreach (Particles.ParticleEmitter particle in ParticleEmitters)
            {
                particle.Emit(ParentObject.LevelParent.Camera.ViewMatrix, ParentObject.LevelParent.Camera.ProjectionMatrix);
            }
        }

        public float? Select(Ray selectionRay)
        {
            return GetBoundingBox().Intersects(selectionRay);
        }
        #endregion

        #region Engine Helper Methods

        #endregion
    }
}