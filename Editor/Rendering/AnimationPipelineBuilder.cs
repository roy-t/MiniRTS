using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline;
using MiniEngine.Pipeline.Extensions;
using MiniEngine.Pipeline.Models.Systems;
using MiniEngine.Pipeline.Particles.Systems;
using MiniEngine.Systems;
using MiniEngine.Telemetry;

namespace MiniEngine.Rendering
{
    public sealed class AnimationPipelineBuilder : APipelineBuilder
    {
        public AnimationPipelineBuilder(IEnumerable<ISystem> systems)
            : base(systems) { }

        public RenderPipeline Build(GraphicsDevice device, IMeterRegistry meterRegistry)
        {
            var pipeline = RenderPipeline.Create(device, meterRegistry);

            pipeline
                .UpdateSystem(this.GetSystem<AveragedParticleSystem>())
                .UpdateSystem(this.GetSystem<AdditiveParticleSystem>())
                .UpdateSystem(this.GetSystem<AnimationSystem>())
                .UpdateSystem(this.GetSystem<UVAnimationSystem>());

            return pipeline;
        }
    }
}
