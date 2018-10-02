using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class AntiAliasingStageExtensions
    {
        public static ModelPipeline AntiAlias(
            this ModelPipeline pipeline,
            PostProcessEffect effect,
            RenderTarget2D sourceTarget,
            RenderTarget2D destinationTarget,
            GBuffer gBuffer,
            float strength)
        {
            var stage = new AntiAliasStage(pipeline.Device, effect, sourceTarget, destinationTarget, gBuffer, strength);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
