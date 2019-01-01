using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.Techniques;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Cameras;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Models.Batches
{
    public sealed class ModelRenderBatch
    {
        private static Matrix[] SharedBoneMatrix;
        private readonly RenderEffect Effect;

        private readonly IReadOnlyList<AModel> Models;
        private readonly IViewPoint ViewPoint;

        public ModelRenderBatch(IReadOnlyList<AModel> models, IViewPoint viewPoint)
        {
            this.Models = models;
            this.ViewPoint = viewPoint;
            this.Effect = new RenderEffect();
        }

        public void Draw(RenderEffectTechniques technique)
        {
            foreach (var modelPose in this.Models)
            {
                this.DrawModel(technique, modelPose.Model, modelPose.Pose, this.ViewPoint);
            }
        }

        private void DrawModel(RenderEffectTechniques technique, Model model, Matrix world, IViewPoint viewPoint)
        {
            var bones = model.Bones.Count;
            if (SharedBoneMatrix is null || SharedBoneMatrix.Length < bones)
            {
                SharedBoneMatrix = new Matrix[bones];
            }

            model.CopyAbsoluteBoneTransformsTo(SharedBoneMatrix);

            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    this.Effect.Wrap(effect);

                    this.Effect.World = SharedBoneMatrix[mesh.ParentBone.Index] * world;
                    this.Effect.View = viewPoint.View;
                    this.Effect.Projection = viewPoint.Projection;

                    this.Effect.Apply(technique);
                }

                mesh.Draw();
            }
        }
    }
}