using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class CombineDiffuseWithLightingStage : IModelPipelineStage
    {
        private readonly GraphicsDevice Device;
        private readonly CombineEffect Effect;
        private readonly RenderTarget2D CombineTarget;
        private readonly GBuffer GBuffer;
        private readonly FullScreenTriangle FullScreenTriangle;

        public CombineDiffuseWithLightingStage(GraphicsDevice device, CombineEffect effect,
                                               RenderTarget2D combineTarget, GBuffer gBuffer)
        {
            this.Device = device;
            this.Effect = effect;
            this.CombineTarget = combineTarget;
            this.GBuffer = gBuffer;
            this.FullScreenTriangle = new FullScreenTriangle();
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch batch)
        {
            this.Device.SetRenderTarget(this.CombineTarget);
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
