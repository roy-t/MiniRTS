using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class ClearStage : IPipelineStage<ParticlePipelineInput>
    {
        private static readonly Color ParticleTargetClearColor = new Color(1.0f, 0, 0, 0);
        private readonly GraphicsDevice Device;
        

        public ClearStage(GraphicsDevice device)
        {
            this.Device = device;
        }

        public void Execute(ParticlePipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.DiffuseTarget);
            this.Device.Clear(ClearOptions.Target, Color.TransparentBlack, 1.0f, 0);

            this.Device.SetRenderTarget(input.GBuffer.ParticleTarget);
            this.Device.Clear(ParticleTargetClearColor);
        }
    }
}
