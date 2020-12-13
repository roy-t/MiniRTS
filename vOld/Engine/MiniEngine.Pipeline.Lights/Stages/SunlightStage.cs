using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class SunlightStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly SunlightSystem SunlightSystem;

        public SunlightStage(GraphicsDevice device, SunlightSystem sunlightSystem)
        {
            this.Device = device;
            this.SunlightSystem = sunlightSystem;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.SunlightSystem.RenderLights(input.Camera, input.GBuffer);
        }
    }
}