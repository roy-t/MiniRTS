using MiniEngine.Pipeline.Lights.Stages;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class ClearStageExtensions
    {
        public static LightingPipeline ClearLightTargets(this LightingPipeline pipeline)
        {
            var stage = new ClearStage(pipeline.Device);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
