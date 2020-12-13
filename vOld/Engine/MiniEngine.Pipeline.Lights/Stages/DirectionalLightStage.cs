using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class DirectionalLightStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly DirectionalLightSystem DirectionalLightSystem;

        public DirectionalLightStage(GraphicsDevice device, DirectionalLightSystem directionalLightSystem)
        {
            this.Device = device;
            this.DirectionalLightSystem = directionalLightSystem;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.DirectionalLightSystem.Render(input.Camera, input.GBuffer);
        }
    }
}