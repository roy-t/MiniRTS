using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;

namespace MiniEngine.Rendering.Batches
{
    public sealed class ModelRenderBatch
    {
        private static Matrix[] SharedBoneMatrix;
        private readonly RenderEffect Effect;

        private readonly IReadOnlyList<ModelPose> Models;
        private readonly IViewPoint ViewPoint;

        public ModelRenderBatch(IReadOnlyList<ModelPose> models, IViewPoint viewPoint)
        {
            this.Models = models;
            this.ViewPoint = viewPoint;
            this.Effect = new RenderEffect();
        }

        public void Draw(Techniques technique)
        {
            foreach (var modelPose in this.Models)
            {
                DrawModel(technique, modelPose.Model, modelPose.Pose, this.ViewPoint);
            }
        }

        private void DrawModel(Techniques technique, Model model, Matrix world, IViewPoint viewPoint)
        {
            var bones = model.Bones.Count;
            if (SharedBoneMatrix == null || SharedBoneMatrix.Length < bones)
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