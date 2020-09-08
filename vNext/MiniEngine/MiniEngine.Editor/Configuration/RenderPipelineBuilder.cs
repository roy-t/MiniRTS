using MiniEngine.Graphics;
using MiniEngine.Systems.Injection;
using MiniEngine.Systems.Pipeline;
using MiniEngine.Systems.Services;

namespace MiniEngine.Editor.Configuration
{
    [Service]
    public sealed class RenderPipelineBuilder
    {
        private readonly PipelineBuilder Builder;

        public RenderPipelineBuilder(PipelineBuilder builder)
        {
            this.Builder = builder;
        }

        public ParallelPipeline Build()
        {
            var builder = this.Builder.Builder();

            builder
                .AddSystem<ClearSystem>(clearSystem => clearSystem.InSequence().Produces("GBuffer", "Cleared"))
                .AddSystem<ClearSystem>(clearSystem => clearSystem.InSequence().Requires("GBuffer", "Cleared").Produces("GBuffer", "Clearer"));

            return builder.Build();
        }
    }
}
