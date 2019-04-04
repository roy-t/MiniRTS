﻿using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Systems;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderWeightedParticlesStage : IPipelineStage<ParticlePipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly ParticleSystem ParticleSystem;

        public RenderWeightedParticlesStage(GraphicsDevice device, ParticleSystem particleSystem)
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
                        
            this.Device.SetRenderTargets(input.GBuffer.FinalTarget);
            this.ParticleSystem.RenderAdditiveParticles(input.Camera, input.GBuffer);
        }
    }
}
