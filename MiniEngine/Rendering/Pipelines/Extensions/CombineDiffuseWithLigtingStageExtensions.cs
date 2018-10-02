using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Extensions
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
