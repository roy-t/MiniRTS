using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Wrappers;
using MiniEngine.Primitives;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class AntiAliasStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly GraphicsDevice Device;
        private readonly FxaaEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        public AntiAliasStage(GraphicsDevice device, FxaaEffect effect, float strength)
        {
            this.Strength = strength;
            this.Device = device;
            this.Effect = effect;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public float Strength { get; }

        public void Execute(ModelPipelineInput input)
        {
            this.Device.SetRenderTarget(input.GBuffer.FinalTarget);
            this.Device.PostProcessState();

            this.Effect.ScaleX = 1.0f / input.GBuffer.CombineTarget.Width;
            this.Effect.ScaleY = 1.0f / input.GBuffer.CombineTarget.Height;
            this.Effect.DiffuseMap = input.GBuffer.CombineTarget;
            this.Effect.NormalMap = input.GBuffer.NormalTarget;
            this.Effect.Strength = this.Strength;

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);
        }
    }
}