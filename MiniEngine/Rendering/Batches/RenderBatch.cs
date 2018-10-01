using System.Collections.Generic;
using MiniEngine.Rendering.Effects;

namespace MiniEngine.Rendering.Batches
{
    // TODO: rename this class
    public sealed class RenderBatch
    {        
        public RenderBatch(ModelRenderBatch opaqueBatch, IReadOnlyList<ModelRenderBatch> transparentBatches)
        {
            this.OpaqueBatch = opaqueBatch;
            this.TransparentBatches = transparentBatches;
        }

        public ModelRenderBatch OpaqueBatch { get; }
        public IReadOnlyList<ModelRenderBatch> TransparentBatches { get; }

        public int TransparentBatchesCount => this.TransparentBatches.Count;

        public void DrawOpaque(Techniques technique)
        {
            this.OpaqueBatch.Draw(technique);
        }

        public void DrawTransparent(int index, Techniques technique)
        {
            this.TransparentBatches[index].Draw(technique);
        }

        public void DrawTransparent(Techniques technique)
        {
            foreach (var batch in this.TransparentBatches)
            {
                batch.Draw(technique);
            }
        }
    }
}
