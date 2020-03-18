using Microsoft.Xna.Framework;
using ModelExtension;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AAnimation
    {
        protected readonly Matrix[] SkinTransforms;

        public AAnimation()
        {
            this.SkinTransforms = new Matrix[Constants.MaxBones];

            for (var i = 0; i < this.SkinTransforms.Length; i++)
            {
                this.SkinTransforms[i] = Matrix.Identity;
            }
        }

        public AModel Target { get; private set; }

        public virtual Matrix[] GetSkinTransforms() => this.SkinTransforms;

        public virtual void SetTarget(AModel target) => this.Target = target;
    }
}
