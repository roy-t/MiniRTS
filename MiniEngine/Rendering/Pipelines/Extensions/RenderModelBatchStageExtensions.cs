using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class RenderModelBatchStageExtensions
    {
        public static ModelPipeline RenderModelBatch(this ModelPipeline pipeline, GBuffer gBuffer)
        {
            var stage = new RenderModelBatchStage(pipeline.Device, gBuffer);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}
