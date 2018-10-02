using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class PointLightStageExtensions
    {
        public static LightingPipeline RenderPointLights(
            this LightingPipeline pipeline,
            PointLightSystem pointLightSystem)
        {
            var stage = new PointLightStage(pointLightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
