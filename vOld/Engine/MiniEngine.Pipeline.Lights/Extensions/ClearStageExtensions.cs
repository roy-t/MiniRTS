using MiniEngine.Pipeline.Lights.Stages;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class ClearStageExtensions
    {
        public static LightingPipeline ClearLightTargets(this LightingPipeline pipeline)
        {
            pipeline.Add(new ClearStage());
            return pipeline;
        }
    }
}
