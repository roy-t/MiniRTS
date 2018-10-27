using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Batches;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.DeviceStates;

namespace MiniEngine.Pipeline.Particles.Stages
{
    public sealed class RenderParticleBatchStage : IParticlePipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly GBuffer GBuffer;

        public RenderParticleBatchStage(GraphicsDevice device, GBuffer gBuffer)
        {
            this.Device = device;
            this.GBuffer = gBuffer;
        }

        public void Execute(PerspectiveCamera camera, ParticleRenderBatch batch)
        {
            this.Device.SetRenderTarget(this.GBuffer.DiffuseTarget);
            using (this.Device.ParticleState())
            {
                batch.Draw(RenderEffectTechniques.Textured);
            }
        }
    }
}