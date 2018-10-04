using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderLightsStageExtensions
    {
        public static ModelPipeline RenderLights(
            this ModelPipeline pipeline,
            LightingPipeline lightingPipeline,
            GBuffer gBuffer)
        {
            var stage = new RenderLightsStage(lightingPipeline, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}