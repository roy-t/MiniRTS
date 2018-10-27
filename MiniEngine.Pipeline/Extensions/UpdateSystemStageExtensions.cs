using MiniEngine.Pipeline.Stages;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Extensions
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