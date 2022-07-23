using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.ScriptEngine.API
{

    

    public class GameObject
    {
        #region Variable holders :)
        private Vector3 _pos;
        private Vector3 _orein;
        private string _name;

        private Vector3 _scale;

        private List<GameObject> _children = new List<GameObject>();

        private List<Particles.ParticleEmitter> _particles;

        private List<Audio.ThreeDSound> _sound;

        private Engine.GameObject ReferenceObject;


        public List<Model> Models;

        public List<SkinnedModel> SkinnedModels = new List<SkinnedModel>();
        #endregion

        #region Properties

        public Vector3 Position { get {
                _pos = ReferenceObject.Location;
                return _pos;
            }
            set {
                _pos = value;
                ReferenceObject.Location = _pos;
            }
        }
      
        public Vector3 Orientation
        {
            get
            {
                _orein = ReferenceObject.Orientation;
                return _pos;
            }
            set
            {
                _orein = value;
                ReferenceObject.Orientation = _orein;
            }
        }
        public Vector3 Scale
        {
            get
            {
               _scale = ReferenceObject.Scale;
                return _scale;
            }
            set
            {
                _scale = value;
                ReferenceObject.Scale = _scale;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                ReferenceObject.Name = value;
            }
        }
        public List<GameObject> Children
        {
            get
            {
                return _children;
            }
            set
            {
                _children = value;
                
            }
        }

        private List<GameObject> buildReferenceTable(List<Engine.GameObject> objects)
        {
            List<GameObject> returnList = new List<GameObject>();
            foreach (Engine.GameObject obj in objects)
            {
                returnList.Add(new GameObject(obj));
            }
            return returnList;
        }

        public List<Particles.ParticleEmitter> Particles
        {
            get {
                _particles = ReferenceObject.ParticleEmitters;
                return _particles;
            }
            set {
                _particles = value;
                ReferenceObject.ParticleEmitters = _particles;
            }
        }
        public List<Audio.ThreeDSound> Sounds
        {
            get
            {
                _sound = ReferenceObject.SoundEmitters;
                return _sound;
            }
            set
            {
                _sound = value;
                ReferenceObject.SoundEmitters = _sound;
            }
        }
       
        #endregion

        #region Methods
        public void PlayAnimation(string modelName, string clip, bool looping = true, int keystart = 0, int keyend = 0, int fps=24)
        {
            foreach (Animation.AnimatedModel model in ReferenceObject.AnimatedModels)
            {
                if (model.Name == modelName)
                foreach (SkinnedModelAnimator.AnimationClip clips in model.GetAnimationClips())
                {
                    if (clips.Name == clip)
                    {
                        model.PlayClip(clips, looping, keystart, keyend, fps);
                        return;
                    }
                }
            }
        }
        public string[] GetAnimations(string modelName)
        {
            foreach (Animation.AnimatedModel mdl in ReferenceObject.AnimatedModels)
            {
                if (mdl.Name == modelName)
                {
                    return mdl.GetAnimationNames();
                }
            }
            return null;
        }

        #endregion

        #region Constructor
        public GameObject(Engine.GameObject reference)
        {
            ReferenceObject = reference;

            _pos = reference.Location;

            _orein = reference.Orientation;

            _scale = reference.Scale;

            _name = reference.Name;

            foreach (Engine.GameObject obj  in reference.Children)
            {
                _children.Add(new GameObject(obj));
            }

            _particles = reference.ParticleEmitters;

            _sound = reference.SoundEmitters;
            

          //  foreach (Animation.AnimatedModel mdl in reference.AnimatedModels)
         //   {
           //     SkinnedModels.Add(new SkinnedModel(mdl));
          //  }

            foreach (BasicModel model in ReferenceObject.Models)
            {
                //Models.Add(new Model(model));
            }

        }

        
        #endregion
    }
}
