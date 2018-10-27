using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class ShadowCastingLightStage : ILightingPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly ShadowCastingLightSystem ShadowCastingLightSystem;

        public ShadowCastingLightStage(GraphicsDevice device, ShadowCastingLightSystem shadowCastingLightSystem)
        {
            this.Device = device;
            this.ShadowCastingLightSystem = shadowCastingLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Device.SetRenderTarget(gBuffer.LightTarget);
            this.ShadowCastingLightSystem.RenderLights(camera, gBuffer);
        }
    }
}