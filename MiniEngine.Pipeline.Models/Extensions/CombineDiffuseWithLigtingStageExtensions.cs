using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class CombineDiffuseWithLigtingStageExtensions
    {
        public static ModelPipeline CombineDiffuseWithLighting(
            this ModelPipeline pipeline,
            CombineEffect effect,
            RenderTarget2D destinationTarget,
            GBuffer gBuffer)
        {
            var stage = new CombineDiffuseWithLightingStage(pipeline.Device, effect, destinationTarget, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}