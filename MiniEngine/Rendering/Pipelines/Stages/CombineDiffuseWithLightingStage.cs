using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class CombineDiffuseWithLightingStage : IModelPipelineStage, IParticlePipelineStage
    {
        private readonly RenderTarget2D DestinationTarget;
        private readonly GraphicsDevice Device;
        private readonly CombineEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly GBuffer GBuffer;

        public CombineDiffuseWithLightingStage(
            GraphicsDevice device,
            CombineEffect effect,
            RenderTarget2D destinationTarget,
            GBuffer gBuffer)
        {
            this.Device = device;
            this.Effect = effect;
            this.DestinationTarget = destinationTarget;
            this.GBuffer = gBuffer;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _)
        {
            Execute();
        }

        public void Execute(PerspectiveCamera camera, ParticleRenderBatch _)
        {
            Execute();
        }

        private void Execute()
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            using (this.Device.PostProcessState())
            {
                this.Effect.DiffuseMap = this.GBuffer.DiffuseTarget;
                this.Effect.LightMap = this.GBuffer.LightTarget;

                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }

            this.Device.SetRenderTarget(null);
        }
    }
}