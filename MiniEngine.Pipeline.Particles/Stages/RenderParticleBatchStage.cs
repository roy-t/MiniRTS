using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.DeviceStates;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderParticleBatchStage : IPipelineStage<ParticlePipelineInput>
    {
        private readonly GraphicsDevice Device;

        public RenderParticleBatchStage(GraphicsDevice device)
        {
            this.Device = device;
        }

        public void Execute(ParticlePipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.DiffuseTarget);
            using (this.Device.ParticleState())
            {
                input.Batch.Draw(RenderEffectTechniques.Textured);
            }
        }
    }
}