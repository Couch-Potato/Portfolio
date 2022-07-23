using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novovu.Xenon.ScriptEngine.API
{
    public class SkinnedModel
    {
        Animation.AnimatedModel animationRef;
        public SkinnedModel(Animation.AnimatedModel reff)
        {
            animationRef = reff;
            foreach (SkinnedModelAnimator.Bone bone in reff.Bones)
            {
                Bones.Add(bone.Name, new Bone(bone));
            }
        }
        public Dictionary<string, Bone> Bones;
    }
}