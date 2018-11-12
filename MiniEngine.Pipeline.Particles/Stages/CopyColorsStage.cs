using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Primitives;
using MiniEngine.Effects.DeviceStates;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class CopyColorsStage : IPipelineStage<ParticlePipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly CopyEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public CopyColorsStage(GraphicsDevice device, CopyEffect effect)
        {
            this.Device = device;
            this.Effect = effect;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void Execute(ParticlePipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            using (this.Device.PostProcessState())
            {
                this.Effect.DiffuseMap = input.GBuffer.DiffuseTarget;
                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }

            this.Device.SetRenderTarget(null);
        }
    }
}