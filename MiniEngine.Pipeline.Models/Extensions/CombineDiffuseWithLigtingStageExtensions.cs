using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Pipeline.Models.Stages;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class CombineDiffuseWithLigtingStageExtensions
    {
        public static ModelPipeline CombineDiffuseWithLighting(
            this ModelPipeline pipeline,
            CombineEffect effect,
            RenderTarget2D destinationTarget)
        {
            var stage = new CombineDiffuseWithLightingStage(pipeline.Device, effect, destinationTarget);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}