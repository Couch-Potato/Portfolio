using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Novovu.Xenon.Editor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Engine
{
    public class Level
    {
        public List<GameObject> Objects = new List<GameObject>();

        public List<Script> Scripts = new List<Script>();

        public Skybox Skybox;

        public Camera Camera = new Camera();

        public bool DefaultMode = false;

        public List<Lighting.IObjectLight> LevelLightingRegister = new List<Lighting.IObjectLight>();

        public Lighting.AmbientLight AmbientLight = new Lighting.AmbientLight();

        public bool UseDefaultLighting = true;

        public List<Audio.StaticSound> StaticSounds = new List<Audio.StaticSound>();

       // public List<UI.UIContainerHost> UIContainers = new List<UI.UIContainerHost>();

        public List<Video.Video> Videos = new List<Video.Video>();

        
        public Lighting.IObjectLight[] GetLightsForObject(Vector3 objLoc)
        {
            Lighting.IObjectLight[] Lights = new Lighting.IObjectLight[3];
            float[] LDist = { float.MaxValue, float.MaxValue, float.MaxValue };
            
            foreach (Lighting.IObjectLight light in LevelLightingRegister)
            {
                float dist = Vector3.Distance(objLoc, light.PositionOffset + light.Parent.Location);
                if (dist <= light.MaxDistance)
                {
                    if (dist < LDist[0])
                    {
                        Lights[0] = light;
                        LDist[0] = dist;
                    }
                    else if (dist < LDist[1])
                    {
                        Lights[1] = light;
                        LDist[1] = dist;
                    }
                    else if (dist < LDist[2])
                    {
                        Lights[2] = light;
                        LDist[2] = dist;
                    }
                }
            }

            return Lights;
        }
        public bool DrawGrid = false;
        public List<IMiscDrawable> DrawableObjects = new List<IMiscDrawable>();
        Grid g;
        public void Render()
        {
            if (g == null)
            {
                g = new Grid(Engine.graphicsDevice, 20);
            }
            
            if (Skybox != null)
            {
                RasterizerState state = new RasterizerState();
                state.CullMode = CullMode.CullClockwiseFace;
                Engine.graphicsDevice.RasterizerState = state;
                Skybox.Render(Camera.ViewMatrix, Camera.ProjectionMatrix, Camera.ViewLocation, Camera.farClipPlane);
                state = new RasterizerState();
                state.CullMode = CullMode.CullCounterClockwiseFace;
                Engine.graphicsDevice.RasterizerState = state;
            }
                
            foreach (GameObject obj in Objects)
            {
                obj.Draw(Camera);
                obj.DoParticles();
            }
           // foreach (UI.UIContainerHost host in UIContainers)
            //{
           //     host.Draw();
           // }
            foreach (Video.Video v in Videos)
            {
                if (v.Started)
                {
                   // Bitmap b = v.Draw();
                 //   Texture2D text = Engine.GetTexture(b);
                   // Engine.SpriteBatch.Begin();
                   // Engine.SpriteBatch.Draw(text, new Microsoft.Xna.Framework.Rectangle(0, 0, 400, 240), Microsoft.Xna.Framework.Color.White);
                   // Engine.SpriteBatch.End();
                }
            }
            if (DrawGrid)
            {
                g.Draw(Camera);
            }
            foreach (IMiscDrawable misc in DrawableObjects)
            {
                misc.Draw(Camera);
            }
        }
        public void CreateSkybox(TextureCube loader, Model mdl, Effect skbx)
        {
            Skybox = new Skybox(mdl, loader,skbx);
           
        }
        public void Update(GameTime time)
        {
            foreach (GameObject obj in Objects)
            {
                obj.Update(time);
            }
           // foreach (UI.UIContainerHost host in UIContainers)
           // {
           //     host.Update();
           // }
            foreach (Video.Video v in Videos)
            {
                v.Update(time);
            }
            List<TaskDelegate> CancelledTasks = new List<TaskDelegate>();
            foreach (TaskDelegate t in Tasks)
            {
                var con = t(time);
                if (con)
                {
                    CancelledTasks.Add(t);
                }
            }
            foreach (TaskDelegate ct in CancelledTasks)
            {
                Tasks.Remove(ct);
            }
        }
        public CollissionDetectors CollissionDetectors = new CollissionDetectors();
        public List<TaskDelegate> Tasks = new List<TaskDelegate>();
        public delegate bool TaskDelegate(GameTime time);
        
    }
    public class CollissionDetectors
    {
        private List<CollisionDetector> detectors = new List<CollisionDetector>();
        public void AddDetector(BoundingBox box, GameObject gameObject)
        {
            CollisionDetector detect = new CollisionDetector();
            detect.BoundingBox = box;
            detect.GameObject = gameObject;
            detectors.Add(detect);
        }
        public void AddDetector(GameObject gameObject)
        {
           
            CollisionDetector detect = new CollisionDetector();
            detect.GameObject = gameObject;
            detectors.Add(detect);
        }
        public void AddDetector(BoundingBox box, BasicModel mdl)
        {
            CollisionDetector detect = new CollisionDetector();
            detect.BoundingBox = box;
            detect.BasicModel = mdl;
            detectors.Add(detect);
        }
        public delegate void OnCollide(CollissionDetectorResult cdr);
        public void AddDetector(BoundingBox box, int cid, OnCollide callback)
        {
            CollisionDetector detection = new CollisionDetector();
            detection.BoundingBox = box;
            detection.OnCollission = new CollisionDetector.OnCollide((CollissionDetectorResult c) =>
            {
                callback(c);
            });
            detection.CollissionLevel = cid;
            detectors.Add(detection);
            //return detection;
        }
        public void RemoveDetector(CollisionDetector collissionDetector)
        {
            detectors.Remove(collissionDetector);
        }

        public CollissionDetectorResult RayIntercepts(Ray r, CollisionDetector.InterceptType type = CollisionDetector.InterceptType.Mouse1Down)
        {
            float Closest = float.MaxValue;
            CollissionDetectorResult closest = new CollissionDetectorResult();
            foreach (CollisionDetector detector in detectors)
            {
                var res = detector.GetResult(r, type);
                
                if (res != null)
                {
                    if (res.Distance < Closest)
                    {
                        Closest = res.Distance;
                        closest = res;
                    }
                }
            }
            return closest;
        }
    }
    public class CollissionDetectorResult
    {
        public bool Collided = false;
        public bool IsModel = false;
        public GameObject GameObject;
        public BasicModel BasicModel;
        public BoundingBox BoundingBox;
        public float Distance = 0f;
        public bool IsGameObject = false;
        public bool IsEvented = false;
        public int CollissionLevel;
        public CollisionDetector CollisionDetector;
        public void Invoke(CollisionDetector.InterceptType type)
        {
            if (CollisionDetector != null)
            {
                InterceptType = type;
                CollisionDetector.OnCollission?.Invoke(this);
            }
        }
        public CollisionDetector.InterceptType InterceptType;
    }
    public class CollisionDetector
    {
        public BoundingBox BoundingBox;
        public enum InterceptType { ObjectCollide, Mouse1Down, MouseHoverStart, Mouse1Up, MouseHoverEnd}
        public GameObject GameObject;
        public BasicModel BasicModel;
        public delegate void OnCollide(CollissionDetectorResult c);
        public OnCollide OnCollission;
        public int CollissionLevel = 1;
        public CollisionDetector()
        {

        }
        public CollissionDetectorResult GetResult(Ray r, InterceptType Type)
        {
           
            if (BoundingBox.GetCorners() != null)
            {
                
                if (GameObject != null)
                {
                    BoundingBox = GameObject.GetBoundingBox();
                }else if (BasicModel != null)
                {
                    BoundingBox = BasicModel.GetBoundingBox();
                }
            }else
            {
                
            }
            float? intsc = r.Intersects(BoundingBox);
            if (intsc != null)
            {
                CollissionDetectorResult res = new CollissionDetectorResult();
                res.IsModel = BasicModel != null ? true : false;
                res.IsEvented = OnCollission != null ? true : false;
                res.IsGameObject = GameObject != null ? true : false;
                res.BasicModel = BasicModel;
                res.GameObject = GameObject;
                res.Distance = (float)intsc;
                res.Collided = true;
                res.BoundingBox = BoundingBox;
                res.InterceptType = Type;
                res.CollisionDetector = this;
                res.CollissionLevel = CollissionLevel;
                if (res.IsEvented)
                {
                    OnCollission(res);
                }
                return res;
            }
            return null;
        }
    }
   
}
