using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class AnimationPlayer
    {
        #region Fields
        Animation.AnimationPlayer AnimationPlayerReference;
        #endregion


        public AnimationPlayer(Animation.AnimationPlayer refer)
        {
            AnimationPlayerReference = refer;

        }

        public bool Looping
        {
            get { return AnimationPlayerReference.Looping; }
            set { AnimationPlayerReference.Looping = value; }
        }
        public float Position
        {
            get { return AnimationPlayerReference.Position; }
            set { AnimationPlayerReference.Position = value; }
        }
        public float EndDuration
        {
            get { return AnimationPlayerReference.EndDuration; }
            set { AnimationPlayerReference.EndDuration = value; }
        }
        public float StartDuration
        {
            get { return AnimationPlayerReference.StartDuration; }
            set { AnimationPlayerReference.StartDuration = value; }
        }
        public float Duration
        {
            get { return AnimationPlayerReference.Duration; }           
        }
        public void Rewind()
        {
            AnimationPlayerReference.Rewind();
        }
       

    }
}
