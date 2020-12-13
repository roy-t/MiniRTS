using System.Collections.Generic;

namespace MiniEngine.Pipeline.Models.Batches
{
    public sealed class ModelBatchList
    {
        public ModelBatchList(ModelRenderBatch opaqueBatch, IReadOnlyList<ModelRenderBatch> transparentBatches)
        {
            this.OpaqueBatch = opaqueBatch;
            this.TransparentBatches = transparentBatches;
        }

        public ModelRenderBatch OpaqueBatch { get; }
        public IReadOnlyList<ModelRenderBatch> TransparentBatches { get; }
    }
}