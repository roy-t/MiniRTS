using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class ClearToAmbientStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly GraphicsDevice Device;

        public ClearToAmbientStage(GraphicsDevice device, AmbientLightSystem ambientLightSystem)
        {
            this.Device = device;
            this.AmbientLightSystem = ambientLightSystem;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.Device.Clear(this.AmbientLightSystem.ComputeAmbientLightZeroAlpha());
        }
    }
}