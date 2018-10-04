using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Units;

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

        public void Execute(PerspectiveCamera camera, ParticleRenderBatch batch, Seconds _)
        {
            this.Device.SetRenderTargets(this.GBuffer.DiffuseTarget, this.GBuffer.NormalTarget, this.GBuffer.DepthTarget);
            using (this.Device.ParticleMRTState())
            {
                batch.Draw(Techniques.MRT);
            }

            this.Device.SetRenderTarget(this.GBuffer.DiffuseTarget);
            //this.Device.Clear(ClearOptions.Target, Color.TransparentBlack, 1, 0);
            using (this.Device.ParticleState())
            {
                batch.Draw(Techniques.Particle);
            }
        }        
    }
}
