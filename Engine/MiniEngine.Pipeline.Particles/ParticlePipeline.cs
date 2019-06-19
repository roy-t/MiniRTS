using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Telemetry;

namespace MiniEngine.Pipeline.Particles
{
    public sealed class ParticlePipeline : APipeline<ParticlePipelineInput>
    {
        private ParticlePipeline(GraphicsDevice device, IMeterRegistry meterRegistry)
            : base(device, meterRegistry, "particle_pipeline")
        {
        }

        public static ParticlePipeline Create(GraphicsDevice device, IMeterRegistry meterRegistry)
            => new ParticlePipeline(device, meterRegistry);
    }
}