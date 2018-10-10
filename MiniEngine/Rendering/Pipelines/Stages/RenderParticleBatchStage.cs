using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
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
                batch.Draw(Techniques.Textured);
            }
        }
    }
}