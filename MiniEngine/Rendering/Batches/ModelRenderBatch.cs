using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;

namespace MiniEngine.Rendering.Batches
{
    public sealed class ModelRenderBatch
    {
        private readonly IReadOnlyList<ModelPose> Models;
        private readonly IViewPoint ViewPoint;

        public ModelRenderBatch(ModelPose model, IViewPoint viewPoint)
                    : this(new[]{model}, viewPoint) { }

        public ModelRenderBatch(IReadOnlyList<ModelPose> models, IViewPoint viewPoint)
        {
            this.Models = models;
            this.ViewPoint = viewPoint;            
        }

        public void Draw(Effect effectOverride = null)
        {
            if (effectOverride != null)
            {
                foreach (var modelPose in this.Models)
                {
                    DrawModel(effectOverride, modelPose.Model, modelPose.Pose, this.ViewPoint);
                }
            }
            else
            {
                foreach (var modelPose in this.Models)
                {
                    DrawModel(modelPose.Model, modelPose.Pose, this.ViewPoint);
                }
            }            
        }

        private static void DrawModel(Model model, Matrix world, IViewPoint viewPoint)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(viewPoint.View);
                    effect.Parameters["Projection"].SetValue(viewPoint.Projection);
                }

                mesh.Draw();
            }
        }

        private static void DrawModel(Effect effectOverride, Model model, Matrix world, IViewPoint viewpoint)
        {
            effectOverride.Parameters["World"].SetValue(world);
            effectOverride.Parameters["View"].SetValue(viewpoint.View);
            effectOverride.Parameters["Projection"].SetValue(viewpoint.Projection);

            foreach (var mesh in model.Meshes)
            {
                var effects = new Effect[mesh.MeshParts.Count];

                for (var i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];

                    CopyTextures(part.Effect, effectOverride);
                    effects[i] = part.Effect;
                    part.Effect = effectOverride;

                }

                mesh.Draw();

                for (var i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];
                    part.Effect = effects[i];
                }
            }
        }

        // TODO: instead of copying all texture parameters we should extend the model type
        // and give it a bundle of textures per mesh part. So we can effectively set those on each shader
        // that needs them (and change the textures if need be).
        // or we could define multiple techniques for the same shader and do without overriding the texture!
        // (Making the shadowmap and colormap effects includes!)
        private static void CopyTextures(Effect source, Effect destination)
        {
            foreach (var parameter in source.Parameters)
            {
                if (parameter.ParameterType == EffectParameterType.Texture2D)
                {
                    destination.Parameters[parameter.Name]?.SetValue(parameter.GetValueTexture2D());
                }
            }
        }
    }
}
