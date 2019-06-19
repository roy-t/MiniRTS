using MiniEngine.Effects;
using MiniEngine.Pipeline.Models.Stages;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class CombineDiffuseWithLigtingStageExtensions
    {
        public static ModelPipeline CombineDiffuseWithLighting(this ModelPipeline pipeline, CombineEffect effect)
        {
            var stage = new CombineDiffuseWithLightingStage(pipeline.Device, effect);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}