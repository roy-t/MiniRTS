using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines.Stages
{
    public sealed class RenderLightsStage : IModelPipelineStage, IParticlePipelineStage
    {
        private readonly LightingPipeline LightingPipeline;
        private readonly GBuffer GBuffer;

        public RenderLightsStage(LightingPipeline lightingPipeline, GBuffer gBuffer)
        {
            this.LightingPipeline = lightingPipeline;
            this.GBuffer = gBuffer;
        }

        private void Execute(PerspectiveCamera camera, Seconds elapsed)
        {
            this.LightingPipeline.Execute(camera, this.GBuffer, elapsed);
        }

        public void Execute(PerspectiveCamera camera, ModelRenderBatch _, Seconds elapsed) => Execute(camera, elapsed);                
        public void Execute(PerspectiveCamera camera, ParticleRenderBatch _, Seconds elapsed) => Execute(camera, elapsed);
    }
}
