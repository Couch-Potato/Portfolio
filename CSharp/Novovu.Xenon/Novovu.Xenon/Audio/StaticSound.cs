using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.Audio
{
    public class StaticSound
    {
        private SoundEffect _sound;
        public void Play()
        {
            if (Instance.State == SoundState.Paused)
            {
                Instance.Resume();
            }
            else
            {
                Instance.Play();
            }
        }

        public bool Looped
        {
            get => Instance.IsLooped;
            set
            {
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
            set
            {
                Instance.Pan = value;
            }
        }
        public SoundEffect Sound
        {
            get
            {
                return _sound;
            }
            set
            {
                _sound = value;
                Instance = value.CreateInstance();
            }
        }
        private SoundEffectInstance Instance;
    }
}
