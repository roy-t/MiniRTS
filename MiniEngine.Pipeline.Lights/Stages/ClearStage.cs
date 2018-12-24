using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class ClearStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly GraphicsDevice Device;

        public ClearStage(GraphicsDevice device)
        {
            this.Device = device;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.TempTarget);
            this.Device.Clear(Color.TransparentBlack);

            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.Device.Clear(Color.TransparentBlack);
        }
    }
}
