using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.PostProcess;
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
            var pipeline = this.Builder.Builder();
            return pipeline
                .System<ClearGBufferSystem>()
                    .InSequence()
                    .Produces("GBuffer", "Clear")
                    .Build()
                .System<GeometrySystem>()
                    .InSequence()
                    .Requires("GBuffer", "Clear")
                    .Produces("GBuffer", "Geometry")
                    .Build()
                .System<CombineSystem>()
                    .InSequence()
                    .Requires("GBuffer", "Geometry")
                    .Produces("Combine", "Geometry")
                    .Build()
                .Build();
        }
    }
}
