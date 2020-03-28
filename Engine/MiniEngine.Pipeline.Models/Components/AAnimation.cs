using System;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AAnimation : IComponent
    {
        protected readonly SkinningData SkinningData;
        public AAnimation(Entity entity, AModel model)
        {
            this.Entity = entity;
            this.Model = model;

            if ((model.Model.Tag as SkinningData) == null)
            {
                throw new ArgumentException("Target does not have skinning data ", nameof(model));
            }
            this.SkinningData = model.Model.Tag as SkinningData;
            this.Model.SkinTransforms = new Matrix[SkinningData.MaxBones];
            for (var i = 0; i < model.SkinTransforms.Length; i++)
            {
                this.Model.SkinTransforms[i] = Matrix.Identity;
            }
        }

        public AModel Model { get; }

        public Entity Entity { get; }

        protected void CopySkinTransformsToModel(Matrix[] skinTransforms)
            => Array.Copy(skinTransforms, 0, this.Model.SkinTransforms, 0, skinTransforms.Length);

        public abstract void Update(Seconds elapsed);
    }
}
