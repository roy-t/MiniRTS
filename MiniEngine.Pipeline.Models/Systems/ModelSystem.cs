using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Batches;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Models.Systems
{
    public sealed class ModelSystem : ISystem
    {
        private readonly List<OpaqueModel> OpaqueModels;
        private readonly List<TransparentModel> TransparentModels;
        private readonly EntityLinker Linker;

        public ModelSystem(EntityLinker linker)
        {
            this.OpaqueModels = new List<OpaqueModel>();
            this.TransparentModels = new List<TransparentModel>();
            this.Linker = linker;
        }

        public ModelBatchList ComputeBatches(IViewPoint viewPoint)
        {
            this.TransparentModels.Clear();
            this.Linker.GetComponents(this.TransparentModels);

            this.OpaqueModels.Clear();
            this.Linker.GetComponents(this.OpaqueModels);

            var transparentBatches = new List<ModelRenderBatch>(this.TransparentModels.Count);

            var transparentModels = SortBackToFront(this.TransparentModels, viewPoint);
            var batches = ComputeBatches(transparentModels, viewPoint);
            foreach (var batch in batches)
            {
                transparentBatches.Add(new ModelRenderBatch(batch, viewPoint));
            }

            return new ModelBatchList(new ModelRenderBatch(this.OpaqueModels, viewPoint), transparentBatches);
        }

        private static IEnumerable<AModel> SortBackToFront(IEnumerable<TransparentModel> models, IViewPoint viewPoint)
        {
            var modeList = new List<AModel>();
            var distanceList = new List<float>();

            foreach (var model in models)
            {
                if (viewPoint.Frustum.Intersects(model.BoundingSphere))
                {
                    var viewPosition = Vector4.Transform(model.BoundingSphere.Center, viewPoint.Frustum.Matrix);
                    // Apply the perspective division
                    var distance = viewPosition.Z / viewPosition.W;

                    InsertBackToFront(modeList, distanceList, model, distance);
                }
            }

            return modeList;
        }

        private static IReadOnlyList<List<AModel>> ComputeBatches(
            IEnumerable<AModel> models,
            IViewPoint viewPoint)
        {
            var batches = new List<List<AModel>>();

            var currentBatch = new List<AModel>();
            var currentBounds = new BoundingRectangle();
            foreach (var model in models)
            {
                var bounds = BoundingRectangle.CreateFromProjectedBoundingBox(
                    model.BoundingBox,
                    viewPoint.Frustum.Matrix);

                if (currentBatch.Count == 0)
                {
                    currentBounds = bounds;
                    currentBatch.Add(model);
                }
                else if (bounds.Intersects(currentBounds))
                {
                    batches.Add(currentBatch);
                    currentBatch = new List<AModel> { model };
                    currentBounds = bounds;
                }
                else
                {
                    currentBatch.Add(model);
                    currentBounds = BoundingRectangle.CreateMerged(currentBounds, bounds);
                }
            }

            if (currentBatch.Count > 0)
            {
                batches.Add(currentBatch);
            }

            return batches;
        }

        private static void InsertBackToFront(
            IList<AModel> models,
            IList<float> distances,
            AModel model,
            float distance)
        {
            for (var i = 0; i < models.Count; i++)
            {
                var distanceToCompare = distances[i];
                if (distance > distanceToCompare)
                {
                    models.Insert(i, model);
                    distances.Insert(i, distance);

                    return;
                }
            }

            models.Add(model);
            distances.Add(distance);
        }
    }
}