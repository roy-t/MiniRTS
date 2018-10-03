using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class AntiAliasStage : IModelPipelineStage, IParticlePipelineStage
    {        
        private readonly GraphicsDevice Device;
        private readonly PostProcessEffect Effect;
        private readonly RenderTarget2D SourceTarget;
        private readonly RenderTarget2D DestinationTarget;
        private readonly GBuffer GBuffer;

        private readonly FullScreenTriangle FullScreenTriangle;

        public AntiAliasStage(
            GraphicsDevice device,
            PostProcessEffect effect,
            RenderTarget2D sourceTarget,
            RenderTarget2D destinationTarget,
            GBuffer gBuffer,
            float strength)
        {
            this.Strength = strength;
            this.Device = device;
            this.Effect = effect;
            this.SourceTarget = sourceTarget;
            this.DestinationTarget = destinationTarget;
            this.GBuffer = gBuffer;

            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public float Strength { get; }        

        private void Execute()
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            using (this.Device.PostProcessState())
            {
                this.Effect.ScaleX = 1.0f / this.SourceTarget.Width;
                this.Effect.ScaleY = 1.0f / this.SourceTarget.Height;
                this.Effect.DiffuseMap = this.SourceTarget;
                this.Effect.NormalMap = this.GBuffer.NormalTarget;
                this.Effect.Strength = this.Strength;

                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _) => Execute();
        public void Execute(PerspectiveCamera camera, ParticleRenderBatch _) => Execute();
    }
}
