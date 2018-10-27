using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;
using MiniEngine.Pipeline.Models;
using MiniEngine.Pipeline.Models.Batches;

namespace MiniEngine.Pipeline.Lights.Stages
{
    public sealed class RenderLightsStage : IModelPipelineStage
    {
        private readonly GBuffer GBuffer;
        private readonly LightingPipeline LightingPipeline;

        public RenderLightsStage(LightingPipeline lightingPipeline, GBuffer gBuffer)
        {
            this.LightingPipeline = lightingPipeline;
            this.GBuffer = gBuffer;
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _) => this.Execute(camera);
        private void Execute(PerspectiveCamera camera) => this.LightingPipeline.Execute(camera, this.GBuffer);
    }
}