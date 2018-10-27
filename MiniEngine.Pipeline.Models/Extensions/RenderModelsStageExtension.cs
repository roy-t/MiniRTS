using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class RenderModelsStageExtension
    {
        public static RenderPipeline RenderModels(
            this RenderPipeline pipeline,
            ModelSystem modelSystem,
            ModelPipeline modelPipeline)
        {
            var stage = new RenderModelsStage(modelSystem, modelPipeline);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}