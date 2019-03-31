using MiniEngine.Pipeline.Lights.Stages;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Extensions
{
    public static class AmbientLightStageExtensions
    {
        public static LightingPipeline RenderAmbientLight(
            this LightingPipeline pipeline,
            AmbientLightSystem ambientLightSystem,
            bool enableSSAO)
        {
            var stage = new AmbientLightStage(pipeline.Device, ambientLightSystem, enableSSAO);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}