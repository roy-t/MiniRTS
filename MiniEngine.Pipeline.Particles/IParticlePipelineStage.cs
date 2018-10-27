using MiniEngine.Pipeline.Particles.Batches;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Particles
{
    public interface IParticlePipelineStage
    {
        void Execute(PerspectiveCamera camera, ParticleRenderBatch batch);
    }
}