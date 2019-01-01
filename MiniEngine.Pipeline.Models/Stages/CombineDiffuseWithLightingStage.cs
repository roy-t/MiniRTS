using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class CombineDiffuseWithLightingStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly CombineEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public CombineDiffuseWithLightingStage(GraphicsDevice device, CombineEffect effect)
        {
            this.Device = device;
            this.Effect = effect;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.CombineTarget);
            using (this.Device.PostProcessState())
            {
                this.Effect.DiffuseMap = input.GBuffer.DiffuseTarget;
                this.Effect.LightMap = input.GBuffer.LightTarget;

                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }
        }
    }
}