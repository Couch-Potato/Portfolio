using Microsoft.Xna.Framework;
using Novovu.Xenon.Editor;
using Novovu.Xenon.Lighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    /// <summary>
    /// Contains the GameObject Primative.
    /// </summary>
    /// <remarks>
    /// This class will hold the texture, the material, the scripts, the components, and the models of it.
    /// </remarks>
    public class GameObject : IBoundHolder,IManipulatable,Editor.ITransformable
    {
        #region Object Properties
        public IParentObject Parent;

        public string Name = "";

        public List<Script> Scripts = new List<Script>();

        public List<Component> Components = new List<Component>();

        public List<BasicModel> Models = new List<BasicModel>();

        public List<Audio.ThreeDSound> SoundEmitters = new List<Audio.ThreeDSound>();

        public List<Particles.ParticleEmitter> ParticleEmitters = new List<Particles.ParticleEmitter>();

        public Vector3 Location = new Vector3(0,0,0);

        public Vector3 Orientation = new Vector3(0,0,0);

        public Vector3 Scale = new Vector3(1,1,1);

        public float Alpha = 1f;

        public enum DrawModes
        {
            Solid,
            Wireframe,

        }

        public DrawModes DrawMode = DrawModes.Solid;

        public List<GameObject> Children = new List<GameObject>();

        public List<Animation.AnimatedModel> AnimatedModels = new List<Animation.AnimatedModel>();

        public Matrix World
        {
            get
            {
                Matrix Rotation = Matrix.CreateRotationX(MathHelper.ToRadians(Orientation.X)) *
                        Matrix.CreateRotationY(MathHelper.ToRadians(Orientation.Y)) *
                        Matrix.CreateRotationZ(MathHelper.ToRadians(Orientation.Z));
                Matrix worldTransform = Rotation * Matrix.CreateTranslation(Location) * Matrix.CreateScale(Scale);
                return worldTransform;
            }
        }
        public BoundingBox GetBoundingBox()
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            float distM = float.MaxValue;
            float distMX = float.MinValue;
            foreach (BasicModel model in Models)
            {
                BoundingBox bx = model.GetBoundingBox();
                if (Vector3.Distance(bx.Min, Location) < distM)
                {
                    distM = Vector3.Distance(bx.Min, Location);
                    min = bx.Min;
                }
                if (Vector3.Distance(bx.Max, Location) > distMX)
                {
                    distMX = Vector3.Distance(bx.Max, Location);
                    max = bx.Max;
                }
            }
            return new BoundingBox(min, max);
        }

        // Properties inherited from its level parent

        public Level LevelParent;


        private List<IObjectLight> _lights = new List<IObjectLight>();

        public void AddLight(IObjectLight light)
        {
            light.Parent = this;
                LevelParent.LevelLightingRegister.Add(light);

            _lights.Add(light);
            
        }

        public List<IObjectLight> Lights
        {
            get
            {
                return _lights;
            }
            set
            {
                if (_lights.Count < value.Count)
                {
                    LevelParent.LevelLightingRegister.Add(value[value.Count - 1]);
                }
                _lights = value;
            }
        }

        private Vector3 Forwards = Vector3.Forward;
        private Vector3 Ups = Vector3.Up;

        string ITransformable.Name { get => Name; set => Name = value; }
        public Vector3 Position { get => Location; set => Location = value; }
        Vector3 ITransformable.Scale { get => Scale; set => Scale = value; }
        public Vector3 Forward { get => World.Forward; set => Forwards = value; }
        public Vector3 Up { get => World.Up; set => Ups = value; }

        public BoundingBox BoundingBox => GetBoundingBox();

        //
        #endregion

        #region Engine Looping Methods
        /*
         * Methods called whenever the game is looping.
         */
        public void Draw(Camera camera)
        {
            foreach (BasicModel model in Models)
            {
                model.Draw(camera, this);
            }
            foreach (Animation.AnimatedModel amodel in AnimatedModels)
            {
                amodel.Draw(camera, this);
            }
            foreach (GameObject child in Children)
            {
                child.Draw(camera);
            }
        }
    
        public void DoParticles()
        {
            foreach (Particles.ParticleEmitter particle in ParticleEmitters)
            {
                particle.Emit(LevelParent.Camera.ViewMatrix, LevelParent.Camera.ProjectionMatrix);
            }
            foreach (BasicModel model in Models)
            {
                model.DoParticles(this);
            }
            foreach (GameObject child in Children)
            {
                child.DoParticles();
            }
        }
        public void Update(GameTime time)
        {
            foreach (Animation.AnimatedModel model in AnimatedModels)
            {
                model.Update(time);
            }
            foreach (GameObject obj in Children)
            {
                obj.Update(time);
            }

            Camera camera = LevelParent.Camera;
            foreach (Audio.ThreeDSound sound in SoundEmitters)
            {
                sound.Update(time,camera, Location);
            }
           
            foreach (GameObject child in Children)
            {
                child.Update(time);
            }

            foreach (Particles.ParticleEmitter particle in ParticleEmitters)
            {
                particle.Update(time, particle.LocationOffset + Location);
            }
        }

        public void SetPosition(Vector3 Pos)
        {
            Location = Pos;
        }
        public Vector3 GetPosition()
        {
            return Location;
        }

        public void SetScale(Vector3 Scale)
        {
            if (Vector3.Min(new Vector3(0,0,0), Scale) == new Vector3(0,0,0))
            {
                this.Scale = Scale;
            }
            
        }

        public Vector3 GetScale()
        {
            return Scale;
        }

        public float? Select(Ray selectionRay)
        {
            float? selection = GetBoundingBox().Intersects(selectionRay);

            return selection;
        }

        #endregion

    }
}
