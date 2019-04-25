using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Stages
{
    public sealed class RenderIconsStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly IconSystem IconSystem;
        private readonly GraphicsDevice Device;

        public RenderIconsStage(IconSystem iconSystem, GraphicsDevice device)
        {
            this.IconSystem = iconSystem;
            this.Device = device;
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.IconSystem.RenderIcons(input.Camera);
        }
    }
}
