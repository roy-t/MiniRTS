using MiniEngine.Rendering.Batches;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IParticlePipelineStage
    {
        void Execute(PerspectiveCamera camera, ParticleRenderBatch batch);
    }
}