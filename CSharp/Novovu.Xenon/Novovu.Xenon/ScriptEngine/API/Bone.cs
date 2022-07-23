using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
namespace Novovu.Xenon.ScriptEngine.API
{
    public class Bone
    {
        public SkinnedModelAnimator.Bone RefBone;


        public Vector3 Scale
        {
            get
            {
                return RefBone.Scale;
            }
            set
            {
                RefBone.Scale = value;
            }

        }

        public Quaternion Rotation
        {
            get
            {
                return RefBone.Rotation;
            }
            set
            {
                RefBone.Rotation = value;
            }
        }
        public Vector3 Translation
        {
            get
            {
                return RefBone.Translation;
            }
            set
            {
                RefBone.Translation = value;
            }
        }

        public Bone(SkinnedModelAnimator.Bone bones)
        {

            RefBone = bones;

        }


    }
}