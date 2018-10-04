using System.Collections.Generic;

namespace MiniEngine.Rendering.Batches
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