using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class ClearToAmbientLightStageExtensions
    {
        public static LightingPipeline ClearToAmbientLight(
            this LightingPipeline pipeline,
            AmbientLightSystem ambientLightSystem)
        {
            var stage = new ClearToAmbientStage(pipeline.Device, ambientLightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}