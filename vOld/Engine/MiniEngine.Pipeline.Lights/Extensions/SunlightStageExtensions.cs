using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class SunlightStageExtensions
    {
        public static LightingPipeline RenderSunlights(this LightingPipeline pipeline, SunlightSystem sunlightSystem)
        {
            var stage = new SunlightStage(pipeline.Device, sunlightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}