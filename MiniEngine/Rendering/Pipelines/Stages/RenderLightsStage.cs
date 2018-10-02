using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderLightsStage : IModelPipelineStage
    {
        private readonly LightingPipeline LightingPipeline;
        private readonly GBuffer GBuffer;

        public RenderLightsStage(LightingPipeline lightingPipeline, GBuffer gBuffer)
        {
            this.LightingPipeline = lightingPipeline;
            this.GBuffer = gBuffer;
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _)
        {
            this.LightingPipeline.Execute(camera, this.GBuffer);
        }
    }
}
