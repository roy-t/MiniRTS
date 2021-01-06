using MiniEngine.Configuration;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.ParticipatingMedia;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Rendering;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Graphics.Visibility;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Pipeline;
using MiniEngine.Systems.Services;
using static MiniEngine.Editor.Configuration.Resource;
using static MiniEngine.Editor.Configuration.Stages;

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
                .System<ComponentFlushSystem>()
                    .Parallel()
                    .Produces(Initialization, Containers)
                    .Build()
                .System<ClearBuffersSystem>()
                    .Parallel()
                    .Produces(Initialization, Buffers)
                    .Build()
                .System<VisibilitySystem>()
                    .Parallel()
                    .Produces(Initialization, Poses)
                    .Requires(Initialization, Containers)
                    .Build()
                .System<GeometrySystem>()
                    .InSequence()
                    .RequiresAll(Initialization)
                    .Produces(RenderGeometry)
                    .Build()
                .System<ShadowMapSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .Produces(RenderShadows, Shadows)
                    .Build()
                .System<CascadedShadowMapSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .Produces(RenderShadows, CascadedShadows)
                    .Build()
                .System<ImageBasedLightSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .Produces(RenderLights, ImageBasedLights)
                    .Build()
                .System<PointLightSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .Produces(RenderLights, PointLights)
                    .Build()
                .System<SpotLightSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .RequiresAll(RenderShadows)
                    .Produces(RenderLights, SpotLights)
                    .Build()
                .System<SunlightSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .RequiresAll(RenderShadows)
                    .Produces(RenderLights, SunLights)
                    .Build()
                .System<SkyboxSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .Produces(Skybox)
                    .Build()
                .System<ParticipatingMediaSystem>()
                    .InSequence()
                    .RequiresAll(RenderShadows)
                    .RequiresAll(RenderLights)
                    .RequiresAll(Skybox)
                    .Produces(ParticipatingMedia)
                    .Build()
                .System<ToneMapSystem>()
                    .InSequence()
                    .RequiresAll(RenderGeometry)
                    .RequiresAll(RenderLights)
                    .RequiresAll(ParticipatingMedia)
                    .RequiresAll(Skybox)
                    .Produces(PostProcess)
                    .Build()
                .Build();
        }
    }
}
