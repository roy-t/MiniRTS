using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Batches
{
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

        public void DrawOpaque(Effect effectOverride = null)
        {
            this.OpaqueBatch.Draw(effectOverride);
        }

        public void DrawTransparent(int index, Effect effectOverride = null)
        {
            this.TransparentBatches[index].Draw(effectOverride);
        }

        public void DrawTransparent(Effect effectOverride = null)
        {
            foreach (var batch in this.TransparentBatches)
            {
                batch.Draw(effectOverride);
            }
        }
    }
}
