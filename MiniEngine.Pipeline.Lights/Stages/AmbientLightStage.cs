using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class AmbientLightStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly GraphicsDevice Device;

        public AmbientLightStage(GraphicsDevice device, AmbientLightSystem ambientLightSystem)
        {
            this.Device = device;
            this.AmbientLightSystem = ambientLightSystem;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.Device.Clear(Color.TransparentBlack);
            this.AmbientLightSystem.Render(input.Camera, input.GBuffer);
        }
    }
}