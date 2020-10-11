using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
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
                    .Produces("GBuffer", "Cleared")
                    .Build()
                .System<GeometrySystem>()
                    .InSequence()
                    .Requires("GBuffer", "Cleared")
                    .Produces("GBuffer", "Geometry")
                    .Build()
                .System<AmbientLightSystem>()
                    .InSequence()
                    .Requires("GBuffer", "Cleared")
                    .Produces("GBuffer", "Ambient Light")
                    .Build()
                .System<CombineSystem>()
                    .InSequence()
                    .Requires("GBuffer", "Geometry")
                    .Requires("GBuffer", "Ambient Light")
                    .Produces("GBuffer", "Combined")
                    .Build()
                .System<BlurSystem>()
                    .InSequence()
                    .Requires("GBuffer", "Combined")
                    .Produces("GBuffer", "PostProcessed")
                    .Build()
                .Build();
        }
    }
}
