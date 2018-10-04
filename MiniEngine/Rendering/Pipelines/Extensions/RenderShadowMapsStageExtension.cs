using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderShadowMapsStageExtension
    {
        public static Pipeline RenderShadowMaps(this Pipeline pipeline, ShadowMapSystem shadowMapSystem)
        {
            var stage = new RenderShadowMapsStage(shadowMapSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}