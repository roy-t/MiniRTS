using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class ShadowCastingLightsStageExtensions
    {
        public static LightingPipeline RenderShadowCastingLights(
            this LightingPipeline pipeline,
            ShadowCastingLightSystem shadowCastingLightSystem)
        {
            var stage = new ShadowCastingLightStage(pipeline.Device, shadowCastingLightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
