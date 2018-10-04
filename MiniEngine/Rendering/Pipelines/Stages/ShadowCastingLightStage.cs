using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
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

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer, Seconds _)
        {
            this.Device.SetRenderTarget(gBuffer.LightTarget);
            this.ShadowCastingLightSystem.RenderLights(camera, gBuffer);
        }
    }
}
