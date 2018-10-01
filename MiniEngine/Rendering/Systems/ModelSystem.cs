using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Systems;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Rendering.Batches;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Utilities.Extensions;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ModelSystem : ISystem
    {
        private readonly Dictionary<Entity, ModelPose> OpaqueModels;
        private readonly Dictionary<Entity, ModelPose> TransparentModels;

        public ModelSystem()
        {
            this.OpaqueModels = new Dictionary<Entity, ModelPose>();
            this.TransparentModels = new Dictionary<Entity, ModelPose>();
        }

        public void Add(Entity entity, Model model, Matrix pose, ModelType modelType = ModelType.Opaque)
        {
            var boundingSphere = model.ComputeBoundingSphere(pose);
            var boundingBox = model.ComputeBoundingBox(pose);

            switch (modelType)
            {
                case ModelType.Opaque:
                    this.OpaqueModels.Add(entity, new ModelPose(model, pose, boundingSphere, boundingBox));
                    break;
                case ModelType.Transparent:
                    this.TransparentModels.Add(entity, new ModelPose(model, pose, boundingSphere, boundingBox));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(modelType), modelType, null);
            }
            
        }

        public bool Contains(Entity entity) => this.OpaqueModels.ContainsKey(entity) || this.TransparentModels.ContainsKey(entity);        

        public string Describe(Entity entity)
        {
            var model = this.OpaqueModels[entity] ?? this.TransparentModels[entity];
            var modelType = this.OpaqueModels.ContainsKey(entity) ? ModelType.Opaque : ModelType.Transparent;

            model.Pose.Decompose(out var scale, out var rotation, out var translation);
        
            return $"model, translation: {translation}, rotation: {rotation}, scale: {scale}, type: {modelType}";
        }

        public void Remove(Entity entity)
        {
            this.OpaqueModels.Remove(entity);
            this.TransparentModels.Remove(entity);
        }


        public RenderBatch ComputeBatches(IViewPoint viewPoint)
        {
            var transparentBatches = new List<ModelRenderBatch>(this.TransparentModels.Count);            

            var transparentModels = SortBackToFront(this.TransparentModels.Values, viewPoint);
            var batches = ComputeBatches(transparentModels, viewPoint);
            foreach (var batch in batches)
            {
                transparentBatches.Add(new ModelRenderBatch(batch, viewPoint));
            }

            return new RenderBatch(new ModelRenderBatch(this.OpaqueModels.Values.ToList(), viewPoint), transparentBatches);
        }

        private static IEnumerable<ModelPose> SortBackToFront(IEnumerable<ModelPose> models, IViewPoint viewPoint)
        {
            var modeList = new List<ModelPose>();
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

        private static IReadOnlyList<List<ModelPose>> ComputeBatches(IEnumerable<ModelPose> models, IViewPoint viewPoint)
        {
            var batches = new List<List<ModelPose>>();

            var currentBatch = new List<ModelPose>();
            var currentBounds = new BoundingRectangle();
            foreach (var model in models)
            {
                var bounds = BoundingRectangle.CreateFromProjectedBoundingBox(model.BoundingBox, viewPoint.Frustum.Matrix);

                if (currentBatch.Count == 0)
                {
                    currentBounds = bounds;
                    currentBatch.Add(model);
                }
                else if (bounds.Intersects(currentBounds))
                {
                    batches.Add(currentBatch);
                    currentBatch = new List<ModelPose> {model};
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

        private static void InsertBackToFront(IList<ModelPose> models, IList<float> distances, ModelPose model, float distance)
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
