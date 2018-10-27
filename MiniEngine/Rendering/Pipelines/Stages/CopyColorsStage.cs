using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class CopyColorsStage : IParticlePipelineStage
    {
        private readonly RenderTarget2D DestinationTarget;
        private readonly GraphicsDevice Device;
        private readonly CopyEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly RenderTarget2D SourceTarget;

        public CopyColorsStage(
            GraphicsDevice device,
            CopyEffect effect,
            RenderTarget2D sourceTarget,
            RenderTarget2D destinationTarget)
        {
            this.Device = device;
            this.Effect = effect;
            this.SourceTarget = sourceTarget;
            this.DestinationTarget = destinationTarget;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void Execute(PerspectiveCamera camera, ParticleRenderBatch batch)
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            using (this.Device.PostProcessState())
            {
                this.Effect.DiffuseMap = this.SourceTarget;
                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }

            this.Device.SetRenderTarget(null);
        }
    }
}