using MiniEngine.Effects;
using MiniEngine.Pipeline.Models.Stages;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class AntiAliasingStageExtensions
    {
        public static ModelPipeline AntiAlias(this ModelPipeline pipeline, FxaaEffect effect, float strength)
        {
            var stage = new AntiAliasStage(pipeline.Device, effect, strength);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}