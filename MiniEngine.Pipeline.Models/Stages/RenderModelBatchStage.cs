using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;
using MiniEngine.Effects.Techniques;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Models.Batches;

namespace MiniEngine.Pipeline.Models.Stages
{
    public sealed class RenderModelBatchStage : IModelPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly GBuffer GBuffer;

        public RenderModelBatchStage(GraphicsDevice device, GBuffer gBuffer)
        {
            this.Device = device;
            this.GBuffer = gBuffer;
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch batch)
        {
            this.Device.SetRenderTargets(
                this.GBuffer.DiffuseTarget,
                this.GBuffer.NormalTarget,
                this.GBuffer.DepthTarget);
            using (this.Device.GeometryState())
            {
                batch.Draw(RenderEffectTechniques.Deferred);
            }
        }
    }
}