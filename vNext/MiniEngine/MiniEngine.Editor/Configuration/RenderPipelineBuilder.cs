using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Rendering;
using MiniEngine.Graphics.Skybox;
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
                .System<ClearBuffersSystem>()
                    .InSequence()
                    .Produces("GBuffer", "Cleared")
                    .Build()
                .System<GeometrySystem>()
                    .InSequence()
                    .Requires("GBuffer", "Cleared")
                    .Produces("GBuffer", "Geometry")
                    .Produces("GBuffer", "Depth")
                    .Build()
                .System<PointLightSystem>()
                    .InSequence()
                    .Requires("GBuffer", "Geometry")
                    .Produces("LBuffer", "Point Light")
                    .Build()
                .System<SkyboxSystem>()
                    .InSequence()
                    .Requires("LBuffer", "Point Light")
                    .Requires("GBuffer", "Depth")
                    .Produces("LBuffer", "Skybox")
                    .Build()
                .System<CombineSystem>()
                    .InSequence()
                    .Requires("LBuffer", "Point Light")
                    .Requires("LBuffer", "Skybox")
                    .Produces("PBuffer", "Combined")
                    .Build()
                .Build();
        }
    }
}
