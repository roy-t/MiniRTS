using MiniEngine.Effects;
using MiniEngine.Pipeline.Particles.Stages;

namespace MiniEngine.Pipeline.Particles.Extensions
{
    public static class CopyColorsStageExtensions
    {
        public static ParticlePipeline CopyColors(this ParticlePipeline pipeline, CopyEffect effect)
        {
            var stage = new CopyColorsStage(pipeline.Device, effect);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}