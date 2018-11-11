using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class ShadowCastingLightStage : IPipelineStage<LightingPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;

        public ShadowCastingLightStage(GraphicsDevice device, ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.Device = device;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
        }

        public void Execute(LightingPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.LightTarget);
            this.ShadowCastingLightSystem.RenderLights(input.Camera, input.GBuffer);
        }
    }
}