using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
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