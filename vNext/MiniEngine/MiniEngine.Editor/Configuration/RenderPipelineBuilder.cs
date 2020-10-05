using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Rendering;
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
                .AddSystem<ClearGBufferSystem>(clearSystem => clearSystem.InSequence()
                                                                  .Produces("GBuffer", "Cleared"))
                .AddSystem<GeometrySystem>(geometrySystem => geometrySystem.InSequence()
                                                                           .Requires("GBuffer", "Cleared")
                                                                           .Produces("Geometry", "Shaded"));

            return builder.Build();
        }
    }
}
