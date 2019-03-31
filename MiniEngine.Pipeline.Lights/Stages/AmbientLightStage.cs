using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class AmbientLightStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly bool EnableSSAO;
        private readonly GraphicsDevice Device;

        public AmbientLightStage(GraphicsDevice device, AmbientLightSystem ambientLightSystem, bool enableSSAO)
        {
            this.Device = device;
            this.AmbientLightSystem = ambientLightSystem;
            this.EnableSSAO = enableSSAO;
        }

        public void Execute(LightingPipelineInput input)
        {
            if (EnableSSAO)
            {
                this.Device.SetRenderTarget(input.GBuffer.BlurTarget);
                this.AmbientLightSystem.RenderSSAO(input.Camera, input.GBuffer.NormalTarget, input.GBuffer.DepthTarget);

                this.Device.SetRenderTarget(input.GBuffer.LightTarget);
                this.AmbientLightSystem.Blur(input.Camera, input.GBuffer.BlurTarget, input.GBuffer.DepthTarget);
            }
            else
            {
                this.Device.SetRenderTarget(input.GBuffer.LightTarget);
                this.AmbientLightSystem.RenderFlat();
            }
        }
    }
}