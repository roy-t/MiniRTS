using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline;
using MiniEngine.Pipeline.Basics.Systems;
using MiniEngine.Pipeline.Extensions;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Pipeline.Particles.Systems;
using MiniEngine.Systems;
using MiniEngine.Telemetry;

namespace MiniEngine.Rendering
{
    public sealed class AnimationPipelineBuilder
    {
        private readonly Resolver<ISystem> Systems;

        public AnimationPipelineBuilder(Resolver<ISystem> systems)
        {
            this.Systems = systems;
        }

        public RenderPipeline Build(GraphicsDevice device, IMeterRegistry meterRegistry)
        {
            var pipeline = RenderPipeline.Create(device, meterRegistry);

            pipeline
                .UpdateSystem(this.Systems.Get<AveragedParticleSystem>())
                .UpdateSystem(this.Systems.Get<AdditiveParticleSystem>())
                .UpdateSystem(this.Systems.Get<AnimationSystem>())
                .UpdateSystem(this.Systems.Get<UVAnimationSystem>());

            return pipeline;
        }
    }
}
