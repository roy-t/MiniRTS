using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class SunlightStageExtensions
    {
        public static LightingPipeline RenderSunlights(this LightingPipeline pipeline, SunlightSystem sunlightSystem)
        {
            var stage = new SunlightStage(sunlightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
