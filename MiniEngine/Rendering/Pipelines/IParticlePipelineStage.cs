using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IParticlePipelineStage
    {
        void Execute(PerspectiveCamera camera, ParticleRenderBatch batch);
    }
}