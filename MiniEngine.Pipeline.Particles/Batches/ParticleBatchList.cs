using System.Collections.Generic;

namespace MiniEngine.Pipeline.Particles.Batches
{
    public sealed class ParticleBatchList
    {
        public ParticleBatchList(IReadOnlyList<ParticleRenderBatch> batches)
        {
            this.Batches = batches;
        }

        public IReadOnlyList<ParticleRenderBatch> Batches { get; }
    }
}