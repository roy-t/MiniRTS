using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderModelsStageExtension
    {
        public static Pipeline RenderModels(
            this Pipeline pipeline,
            ModelSystem modelSystem,
            ModelPipeline modelPipeline)
        {
            var stage = new RenderModelsStage(modelSystem, modelPipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}