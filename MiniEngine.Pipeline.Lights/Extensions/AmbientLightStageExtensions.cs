using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class AmbientLightStageExtensions
    {
        public static LightingPipeline RenderAmbientLight(
            this LightingPipeline pipeline,
            AmbientLightSystem ambientLightSystem)
        {
            var stage = new AmbientLightStage(pipeline.Device, ambientLightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}