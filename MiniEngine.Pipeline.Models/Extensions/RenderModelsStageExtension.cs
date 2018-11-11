using MiniEngine.Pipeline.Models.Stages;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Extensions
{
    public static class RenderModelsStageExtension
    {
        public static RenderPipeline RenderModels(
            this RenderPipeline pipeline,
            ModelSystem modelSystem,
            ModelPipeline modelPipeline,
            GBuffer gBuffer)
        {
            var stage = new RenderModelsStage(modelSystem, modelPipeline, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}