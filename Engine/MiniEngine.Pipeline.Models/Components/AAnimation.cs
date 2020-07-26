using Microsoft.Xna.Framework;
using MiniEngine.ContentProcessors;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AAnimation : IComponent
    {
        public AAnimation(Entity entity, SkinningData skinningData)
        {
            this.Entity = entity;

            this.SkinningData = skinningData;
            this.SkinTransforms = new Matrix[SkinningData.MaxBones];
            for (var i = 0; i < this.SkinTransforms.Length; i++)
            {
                this.SkinTransforms[i] = Matrix.Identity;
            }
        }

        public Entity Entity { get; }

        public SkinningData SkinningData { get; }

        public Matrix[] SkinTransforms { get; }

        public abstract void Update(Seconds elapsed);
    }
}
