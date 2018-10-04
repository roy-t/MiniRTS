using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Rendering.Systems;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class ClearToAmbientStage : ILightingPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly AmbientLightSystem AmbientLightSystem;

        public ClearToAmbientStage(GraphicsDevice device, AmbientLightSystem ambientLightSystem)
        {
            this.Device = device;
            this.AmbientLightSystem = ambientLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer, Seconds _)
        {
            this.Device.SetRenderTarget(gBuffer.LightTarget);
            this.Device.Clear(this.AmbientLightSystem.ComputeAmbientLightZeroAlpha());
        }
    }
}
