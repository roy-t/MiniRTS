using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class PointLightStage : ILightingPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly PointLightSystem PointLightSystem;

        public PointLightStage(GraphicsDevice device, PointLightSystem pointLightSystem)
        {
            this.Device = device;
            this.PointLightSystem = pointLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer, Seconds _)
        {
            this.Device.SetRenderTarget(gBuffer.LightTarget);
            this.PointLightSystem.Render(camera, gBuffer);
        }
    }
}
