using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class DirectionalLightStage : ILightingPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly DirectionalLightSystem DirectionalLightSystem;

        public DirectionalLightStage(GraphicsDevice device, DirectionalLightSystem directionalLightSystem)
        {
            this.Device = device;
            this.DirectionalLightSystem = directionalLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Device.SetRenderTarget(gBuffer.LightTarget);
            this.DirectionalLightSystem.Render(camera, gBuffer);
        }
    }
}