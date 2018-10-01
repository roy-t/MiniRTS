﻿using System.Collections.Generic;

namespace MiniEngine.Rendering.Batches
{
    public sealed class RenderBatchList
    {        
        public RenderBatchList(ModelRenderBatch opaqueBatch, IReadOnlyList<ModelRenderBatch> transparentBatches)
        {
            this.OpaqueBatch = opaqueBatch;
            this.TransparentBatches = transparentBatches;
        }

        public ModelRenderBatch OpaqueBatch { get; }
        public IReadOnlyList<ModelRenderBatch> TransparentBatches { get; }       
    }
}