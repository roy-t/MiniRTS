using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class RenderShadowMapsStageExtension
    {
        public static RenderPipeline RenderShadowMaps(this RenderPipeline pipeline, ShadowMapSystem shadowMapSystem)
        {
            var stage = new RenderShadowMapsStage(shadowMapSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}