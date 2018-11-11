using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Primitives;
using MiniEngine.Effects.DeviceStates;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class CombineDiffuseWithLightingStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly RenderTarget2D DestinationTarget;
        private readonly GraphicsDevice Device;
        private readonly CombineEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public CombineDiffuseWithLightingStage(
            GraphicsDevice device,
            CombineEffect effect,
            RenderTarget2D destinationTarget)
        {
            this.Device = device;
            this.Effect = effect;
            this.DestinationTarget = destinationTarget;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Device.SetRenderTarget(this.DestinationTarget);
            using (this.Device.PostProcessState())
            {
                this.Effect.DiffuseMap = input.GBuffer.DiffuseTarget;
                this.Effect.LightMap = input.GBuffer.LightTarget;

                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }

            this.Device.SetRenderTarget(null);
        }
    }
}