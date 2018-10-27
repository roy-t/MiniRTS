using MiniEngine.Pipeline;
using MiniEngine.Rendering.Pipelines.Stages;
using MiniEngine.Systems;

namespace MiniEngine.Rendering.Pipelines.Extensions
{
    public static class UpdateSystemStageExtensions
    {
        public static RenderPipeline UpdateSystem(this RenderPipeline pipeline, IUpdatableSystem system)
        {
            var stage = new UpdateSystemStage(system);
            pipeline.Add(stage);
            return pipeline;
        }
    }
}