using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Systems;

namespace MiniEngine.Pipeline.Debug.Stages
{
    public sealed class RenderDebugLinesStage : IPipelineStage<RenderPipelineInput>
    {
        private readonly LineSystem LineSystem;
        private readonly GraphicsDevice Device;

        public RenderDebugLinesStage(LineSystem lineSystem, GraphicsDevice device)
        {
            this.LineSystem = lineSystem;
            this.Device = device;
        }

        public void Execute(RenderPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.LineSystem.RenderLines(input.Camera, input.GBuffer);
        }
    }
}
