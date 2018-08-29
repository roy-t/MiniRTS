﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Systems;
using System.Collections.Generic;
using System.Linq;
using MiniEngine.Rendering.Batches;

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
            switch (modelType)
            {
                case ModelType.Opaque:
                    this.OpaqueModels.Add(entity, new ModelPose(model, pose));
                    break;
                case ModelType.Transparent:
                    this.TransparentModels.Add(entity, new ModelPose(model, pose));
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
            var translation = model.Pose.Translation;
            var rotation = model.Pose.Rotation;
            var scale = model.Pose.Scale;

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
            foreach (var modelPose in transparentModels)
            {
                transparentBatches.Add(new ModelRenderBatch(modelPose, viewPoint));
            }

            return new RenderBatch(new ModelRenderBatch(this.OpaqueModels.Values.ToList(), viewPoint), transparentBatches);
        }

        private static IEnumerable<ModelPose> SortBackToFront(IEnumerable<ModelPose> models, IViewPoint viewPoint)
        {
            // TODO: sort models back to front in relation to the camera
            return models;
        }
    }
}
