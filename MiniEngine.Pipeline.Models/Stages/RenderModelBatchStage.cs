using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.DeviceStates;

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
            using (this.Device.GeometryState())
            {
                input.Batch.Draw(RenderEffectTechniques.Deferred);
            }
        }
    }
}