using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Systems;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class Render2DOutlineStage: IPipelineStage<RenderPipelineInput>
    {
        private readonly OutlineSystem OutlineSystem;
        private readonly GraphicsDevice Device;

        public Render2DOutlineStage(OutlineSystem outlineSystem, GraphicsDevice device)
        {
            this.OutlineSystem = outlineSystem;
            this.Device = device;
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.OutlineSystem.Render2DOverlay(input.Camera);
        }
    }
}
