using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Effects;
using MiniEngine.Primitives;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Models.Batches;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class AntiAliasStage : IModelPipelineStage
    {
        private readonly RenderTarget2D DestinationTarget;
        private readonly GraphicsDevice Device;
        private readonly FxaaEffect Effect;

        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly GBuffer GBuffer;
        private readonly RenderTarget2D SourceTarget;

        public AntiAliasStage(
            GraphicsDevice device,
            FxaaEffect effect,
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

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _) => this.Execute();

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
    }
}