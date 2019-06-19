using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
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