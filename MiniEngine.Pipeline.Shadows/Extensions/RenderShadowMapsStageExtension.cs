using MiniEngine.Pipeline.Shadows.Stages;
using MiniEngine.Pipeline.Shadows.Systems;

namespace MiniEngine.Pipeline.Shadows.Extensions
{
    public static class RenderShadowMapsStageExtension
    {
        public static ShadowPipeline RenderShadowMaps(this ShadowPipeline pipeline, ShadowMapSystem shadowMapSystem)
        {
            var stage = new RenderShadowMapsStage(shadowMapSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}