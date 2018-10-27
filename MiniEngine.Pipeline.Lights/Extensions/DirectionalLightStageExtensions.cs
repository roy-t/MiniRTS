using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class DirectionalLightStageExtensions
    {
        public static LightingPipeline RenderDirectionalLights(
            this LightingPipeline pipeline,
            DirectionalLightSystem directionalLightSystem)
        {
            var stage = new DirectionalLightStage(pipeline.Device, directionalLightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}