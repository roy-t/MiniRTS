using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Batches;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;

namespace MiniEngine.Pipeline.Models.Systems
{

    public class ModelPose
    {
        public ModelPose(AModel model, Pose pose)
        {
            this.Model = model;
            this.Pose = pose;
        }

        public AModel Model { get; }
        public Pose Pose { get; }
    }

    // TODO: completely redo batches
    // use one big list with ranges on them for memory purposes. (Everything is an AModel anyway)

    public sealed class ModelSystem : ISystem
    {
        private readonly IComponentContainer<OpaqueModel> OpaqueModels;
        private readonly IComponentContainer<TransparentModel> TransparentModels;
        private readonly IComponentContainer<Pose> Poses;
        private readonly List<ModelPose> OpaqueModelBatchList;

        public ModelSystem(
            IComponentContainer<OpaqueModel> opaqueModels,
            IComponentContainer<TransparentModel> transparentModels,
            IComponentContainer<Pose> poses)
        {
            this.OpaqueModels = opaqueModels;
            this.TransparentModels = transparentModels;
            this.Poses = poses;

            this.OpaqueModelBatchList = new List<ModelPose>();
        }

        public ModelBatchList ComputeBatches(IViewPoint viewPoint, TextureCube skybox)
        {
            var transparentBatches = new List<ModelRenderBatch>(this.TransparentModels.Count);

            var transparentModels = this.SortBackToFront(this.TransparentModels, viewPoint);
            var batches = this.ComputeBatches(transparentModels, viewPoint);
            for (var i = 0; i < batches.Count; i++)
            {
                transparentBatches.Add(new ModelRenderBatch(batches[i], viewPoint, skybox));
            }

            this.OpaqueModelBatchList.Clear();
            for (var i = 0; i < this.OpaqueModels.Count; i++)
            {
                var model = this.OpaqueModels[i];
                var pose = this.Poses.Get(model.Entity);

                var modelPose = new ModelPose(model, pose);
                this.OpaqueModelBatchList.Add(modelPose);

            }

            return new ModelBatchList(new ModelRenderBatch(this.OpaqueModelBatchList, viewPoint, skybox), transparentBatches);
        }

        private List<AModel> SortBackToFront(IComponentContainer<TransparentModel> models, IViewPoint viewPoint)
        {
            var modeList = new List<AModel>();
            var distanceList = new List<float>();

            for (var i = 0; i < models.Count; i++)
            {
                var model = models[i];
                var pose = this.Poses.Get(model.Entity);

                // TODO: replace with bounds system?
                model.Model.ComputeExtremes(pose.Matrix, out var min, out var max);
                var boundingSphere = new BoundingSphere(Vector3.Lerp(min, max, 0.5f), Vector3.Distance(min, max) * 0.5f);

                if (viewPoint.Frustum.Intersects(boundingSphere))
                {
                    var viewPosition = Vector4.Transform(boundingSphere.Center, viewPoint.Frustum.Matrix);
                    // Apply the perspective division
                    var distance = viewPosition.Z / viewPosition.W;

                    InsertBackToFront(modeList, distanceList, model, distance);
                }
            }

            return modeList;
        }

        private IReadOnlyList<List<ModelPose>> ComputeBatches(List<AModel> models, IViewPoint viewPoint)
        {
            var batches = new List<List<ModelPose>>();

            var currentBatch = new List<ModelPose>();
            var currentBounds = new BoundingRectangle();

            for (var i = 0; i < models.Count; i++)
            {
                var model = models[i];
                var pose = this.Poses.Get(model.Entity);
                var modelPose = new ModelPose(model, pose);
                model.Model.ComputeExtremes(pose.Matrix, out var min, out var max);

                var boundingBox = new BoundingBox(min, max);

                var bounds = BoundingRectangle.CreateFromProjectedBoundingBox(
                    boundingBox,
                    viewPoint);

                if (currentBatch.Count == 0)
                {
                    currentBounds = bounds;
                    currentBatch.Add(modelPose);
                }
                else if (bounds.Intersects(currentBounds))
                {
                    batches.Add(currentBatch);
                    currentBatch = new List<ModelPose> { modelPose };
                    currentBounds = bounds;
                }
                else
                {
                    currentBatch.Add(modelPose);
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