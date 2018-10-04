using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
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

        public void Execute(PerspectiveCamera camera, ModelRenderBatch batch, Seconds _)
        {
            this.Device.SetRenderTargets(this.GBuffer.DiffuseTarget, this.GBuffer.NormalTarget, this.GBuffer.DepthTarget);
            using (this.Device.GeometryState())
            {
                batch.Draw(Techniques.MRT);
            }
        }
    }
}
