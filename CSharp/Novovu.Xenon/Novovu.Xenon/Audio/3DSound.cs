using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Novovu.Xenon.Engine;

namespace Novovu.Xenon.Audio
{
    public class ThreeDSound
    {
        private SoundEffect _sound;

        public void Update(GameTime span, Camera camera, Vector3 Location)
        {
            //Logging.Logger.Log(Logging.Logger.LogTypes.Message, this, camera.ViewLocation.ToString());
            AudioListener.Position = camera.ViewLocation;
            AudioListener.Forward = camera.LookAtVector;
            AudioEmitter.Position = Location;
            Instance.Volume = doSoundPhysics(getDistanceFromObject(camera.ViewLocation, Location));
            //Instance.Pan = getSoundPan(camera.ViewLocation, Location, camera.LookAtVector);
            Instance.Apply3D(AudioListener, AudioEmitter);
        }

        public float FallOffMinDistance = 20f;
        public float MaxDistance = 200f;
        public float MaxVolume = 10f;
        private float getDistanceFromObject(Vector3 o1, Vector3 o2)
        {
            return Vector3.Distance(o1, o2);
        }

        private float doSoundPhysics(float distance)
        {
            
            if (distance < MaxDistance && distance > MaxDistance *-1)
            {
                if (distance < 0)
                {
                    distance = distance * -1;
                }
                float max = MaxDistance;
                float min = FallOffMinDistance;
                float coeff = ((distance - max) / (min - max));
                if ((float)Math.Pow(coeff, 2.0) > MaxVolume)
                {
                    return MaxVolume;
                }
                return (float)Math.Pow(coeff, 2.0);
            }else
            {
                return 0;
            }
            
        }

        private float getSoundPan(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            return v2.X/v3.X;
        }

        public void Play()
        {
            if (Instance.State == SoundState.Paused)
            {
                Instance.Resume();
            }else
            {
                Instance.Play();
            }
        }

        public bool Looped
        {
            get => Instance.IsLooped;
            set {
                Instance.IsLooped = value;
            }
        }

        public void Stop()
        {
            Instance.Stop();
        }
        public void Pause()
        {
            Instance.Pause();
        }
        public float Volume
        {
            get => Instance.Volume;
            set
            {
                Instance.Volume = value;
            }
        }
        public float Pitch
        {
            get => Instance.Pitch;
            set
            {
                Instance.Pitch = value;
            }
        }

        public float Pan
        {
            get => Instance.Pan;
            set {
                Instance.Pan = value;
            }
        }
        public SoundEffect Sound {
            get
            {
                return _sound;
            }
            set {
                _sound = value;
                Instance = value.CreateInstance();
            }
        }

        private AudioListener AudioListener = new AudioListener();
        private AudioEmitter AudioEmitter =  new AudioEmitter();

        private SoundEffectInstance Instance;
    }
}
