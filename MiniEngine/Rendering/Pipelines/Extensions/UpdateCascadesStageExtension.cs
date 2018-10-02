using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class UpdateCascadesStageExtension
    {
        public static Pipeline UpdateCascades(this Pipeline pipeline, SunlightSystem sunlightSystem)
        {
            var stage = new UpdateCascadesStage(sunlightSystem);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
