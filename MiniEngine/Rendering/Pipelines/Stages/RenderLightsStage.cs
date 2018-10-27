using MiniEngine.Rendering.Batches;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderLightsStage : IModelPipelineStage, IParticlePipelineStage
    {
        private readonly GBuffer GBuffer;
        private readonly LightingPipeline LightingPipeline;

        public RenderLightsStage(LightingPipeline lightingPipeline, GBuffer gBuffer)
        {
            this.LightingPipeline = lightingPipeline;
            this.GBuffer = gBuffer;
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _) => this.Execute(camera);

        public void Execute(PerspectiveCamera camera, ParticleRenderBatch _) => this.Execute(camera);

        private void Execute(PerspectiveCamera camera) => this.LightingPipeline.Execute(camera, this.GBuffer);
    }
}