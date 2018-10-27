using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Primitives;
using MiniEngine.Pipeline.Models;

namespace MiniEngine.Pipeline.Lights.Extensions
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