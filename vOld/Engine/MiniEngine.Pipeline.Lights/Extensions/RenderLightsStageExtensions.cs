using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Models;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class RenderLightsStageExtensions
    {
        public static ModelPipeline RenderLights(
            this ModelPipeline pipeline,
            LightingPipeline lightingPipeline)
        {
            var stage = new RenderLightsStage(lightingPipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}