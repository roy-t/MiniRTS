using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;
using MiniEngine.Pipeline.Lights.Systems;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class ClearToAmbientStage : ILightingPipelineStage
    {
        private readonly AmbientLightSystem AmbientLightSystem;
        private readonly GraphicsDevice Device;

        public ClearToAmbientStage(GraphicsDevice device, AmbientLightSystem ambientLightSystem)
        {
            this.Device = device;
            this.AmbientLightSystem = ambientLightSystem;
        }

        public void Execute(PerspectiveCamera camera, GBuffer gBuffer)
        {
            this.Device.SetRenderTarget(gBuffer.LightTarget);
            this.Device.Clear(this.AmbientLightSystem.ComputeAmbientLightZeroAlpha());
        }
    }
}