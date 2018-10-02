using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
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
