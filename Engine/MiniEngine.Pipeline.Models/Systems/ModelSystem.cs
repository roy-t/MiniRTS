using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Batches;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Models.Systems
{
    public sealed class ModelSystem : ISystem
    {
        private readonly IComponentContainer<OpaqueModel> OpaqueModels;
        private readonly IComponentContainer<TransparentModel> TransparentModels;
        private readonly List<OpaqueModel> OpaqueModelBatchList;

        public ModelSystem(IComponentContainer<OpaqueModel> opaqueModels, IComponentContainer<TransparentModel> transparentModels)
        {
            this.OpaqueModels = opaqueModels;
            this.TransparentModels = transparentModels;

            this.OpaqueModelBatchList = new List<OpaqueModel>();
        }

        public ModelBatchList ComputeBatches(IViewPoint viewPoint, TextureCube skybox)
        {
            var transparentBatches = new List<ModelRenderBatch>(this.TransparentModels.Count);

            var transparentModels = SortBackToFront(this.TransparentModels, viewPoint);
            var batches = ComputeBatches(transparentModels, viewPoint);
            for (var i = 0; i < batches.Count; i++)
            {
                transparentBatches.Add(new ModelRenderBatch(batches[i], viewPoint, skybox));
            }

            this.OpaqueModelBatchList.Clear();
            for(var i = 0; i < this.OpaqueModels.Count; i++)
            {
                this.OpaqueModelBatchList.Add(this.OpaqueModels[i]);
            }

            return new ModelBatchList(new ModelRenderBatch(this.OpaqueModelBatchList, viewPoint, skybox), transparentBatches);
        }

        private static List<AModel> SortBackToFront(IComponentContainer<TransparentModel> models, IViewPoint viewPoint)
        {
            var modeList = new List<AModel>();
            var distanceList = new List<float>();

            for (var i = 0; i < models.Count; i++)
            {
                var model = models[i];

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

        private static IReadOnlyList<List<AModel>> ComputeBatches(List<AModel> models, IViewPoint viewPoint)
        {
            var batches = new List<List<AModel>>();

            var currentBatch = new List<AModel>();
            var currentBounds = new BoundingRectangle();

            for (var i = 0; i < models.Count; i++)
            {
                var model = models[i];

                var bounds = BoundingRectangle.CreateFromProjectedBoundingBox(
                    model.BoundingBox,
                    viewPoint);

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