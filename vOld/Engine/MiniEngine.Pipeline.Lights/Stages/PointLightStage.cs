using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class PointLightStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly PointLightSystem PointLightSystem;

        public PointLightStage(GraphicsDevice device, PointLightSystem pointLightSystem)
        {
            this.Device = device;
            this.PointLightSystem = pointLightSystem;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.PointLightSystem.Render(input.Camera, input.GBuffer);
        }
    }
}