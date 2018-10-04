using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Rendering.Pipelines
{
    public interface IParticlePipelineStage
    {
        void Execute(PerspectiveCamera camera, ParticleRenderBatch batch, Seconds elapsed);
    }
}
