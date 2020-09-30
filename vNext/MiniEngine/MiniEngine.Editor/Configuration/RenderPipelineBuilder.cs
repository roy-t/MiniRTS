using MiniEngine.Configuration;
using MiniEngine.Graphics;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Models;
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
                .AddSystem<ClearSystem>(clearSystem => clearSystem.InSequence()
                                                                  .Produces("GBuffer", "Cleared"))
                .AddSystem<ModelSystem>(modelSystem => modelSystem.InSequence()
                                                                  .Requires("GBuffer", "Cleared")
                                                                  .Produces("Models", "Shaded"))
                .AddSystem<GeometrySystem>(geometrySystem => geometrySystem.InSequence()
                                                                           .Requires("GBuffer", "Cleared")
                                                                           .Produces("Geometry", "Shaded"));

            return builder.Build();
        }
    }
}
