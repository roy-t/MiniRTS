using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Pipeline.Particles.Stages;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class CopyColorsStageExtensions
    {
        public static ParticlePipeline CopyColors(
            this ParticlePipeline pipeline,
            CopyEffect effect,
            RenderTarget2D sourceTarget,
            RenderTarget2D destinationTarget)
        {
            var stage = new CopyColorsStage(pipeline.Device, effect, sourceTarget, destinationTarget);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}