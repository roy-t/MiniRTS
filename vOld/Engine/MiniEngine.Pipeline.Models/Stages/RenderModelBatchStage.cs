using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Effects.Techniques;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class RenderModelBatchStage : IPipelineStage<ModelPipelineInput>
    {
        private readonly GraphicsDevice Device;

        public RenderModelBatchStage(GraphicsDevice device)
        {
            this.Device = device;
        }

        public void Execute(ModelPipelineInput input)
        {
            this.Device.SetRenderTargets(
                input.GBuffer.DiffuseTarget,
                input.GBuffer.NormalTarget,
                input.GBuffer.DepthTarget);

            this.Device.GeometryState();
            input.Batch.Draw(RenderEffectTechniques.Deferred);
        }
    }
}