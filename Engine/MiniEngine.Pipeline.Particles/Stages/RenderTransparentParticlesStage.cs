using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderTransparentParticlesStage : IPipelineStage<ParticlePipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly TransparentParticleSystem ParticleSystem;

        public RenderTransparentParticlesStage(GraphicsDevice device, TransparentParticleSystem particleSystem)
        {
            this.Device = device;
            this.ParticleSystem = particleSystem;
        }

        public void Execute(ParticlePipelineInput input)
        {
            this.Device.SetRenderTargets(input.GBuffer.DiffuseTarget, input.GBuffer.ParticleTarget);
            this.ParticleSystem.RenderParticleWeights(input.Camera, input.GBuffer);

            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.ParticleSystem.AverageParticles(input.GBuffer.DiffuseTarget, input.GBuffer.ParticleTarget);
        }
    }
}
