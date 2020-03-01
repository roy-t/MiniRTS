using Microsoft.Xna.Framework;
using ModelExtension;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AAnimation
    {
        protected readonly Matrix[] BoneTransforms;

        public AAnimation()
        {
            this.BoneTransforms = new Matrix[Constants.MaxBones];

            for (var i = 0; i < this.BoneTransforms.Length; i++)
            {
                this.BoneTransforms[i] = Matrix.Identity;
            }
        }

        public AModel Target { get; private set; }

        public virtual Matrix[] GetBoneTransforms() => this.BoneTransforms;

        public virtual void SetTarget(AModel target) => this.Target = target;
    }
}
