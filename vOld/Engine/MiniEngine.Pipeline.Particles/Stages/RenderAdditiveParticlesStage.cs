using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderAdditiveParticlesStage : IPipelineStage<ParticlePipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly AdditiveParticleSystem ParticleSystem;

        public RenderAdditiveParticlesStage(GraphicsDevice device, AdditiveParticleSystem additiveParticleSystem)
        {
            this.Device = device;
            this.ParticleSystem = additiveParticleSystem;
        }

        public void Execute(ParticlePipelineInput input)
        {                        
            this.Device.SetRenderTargets(input.GBuffer.FinalTarget);
            this.ParticleSystem.RenderAdditiveParticles(input.Camera, input.GBuffer);
        }
    }
}
