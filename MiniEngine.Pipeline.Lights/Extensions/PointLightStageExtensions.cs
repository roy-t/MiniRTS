using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class PointLightStageExtensions
    {
        public static LightingPipeline RenderPointLights(
            this LightingPipeline pipeline,
            PointLightSystem pointLightSystem)
        {
            var stage = new PointLightStage(pipeline.Device, pointLightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}