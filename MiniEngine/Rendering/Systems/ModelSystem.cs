using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Systems;
using System.Collections.Generic;

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

        public bool Contains(Entity entity) => this.OpaqueModels.ContainsKey(entity);        

        public string Describe(Entity entity)
        {
            var model = this.OpaqueModels[entity];
            var translation = model.Pose.Translation;
            var rotation = model.Pose.Rotation;
            var scale = model.Pose.Scale;

            return $"model, translation: {translation}, rotation: {rotation}, scale: {scale}";
        }

        public void Remove(Entity entity)
        {
            this.OpaqueModels.Remove(entity);
        }       

        public void DrawOpaqueModels(IViewPoint viewPoint)
        {
            foreach (var modelPose in this.OpaqueModels.Values)
            {
                DrawModel(modelPose.Model, modelPose.Pose, viewPoint);
            }            
        }        

        public void DrawOpaqueModels(IViewPoint viewPoint, Effect effectOverride)
        {
            foreach (var modelPose in this.OpaqueModels.Values)
            {
                DrawModel(effectOverride, modelPose.Model, modelPose.Pose, viewPoint);
            }
        }

        public void DrawTransparentModels(IViewPoint viewPoint, Effect effectOverride)
        {
            foreach (var modelPose in this.TransparentModels.Values)
            {
                DrawModel(effectOverride, modelPose.Model, modelPose.Pose, viewPoint);
            }
        }

        private static void DrawModel(Model model, Matrix world, IViewPoint viewPoint)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(viewPoint.View);
                    effect.Parameters["Projection"].SetValue(viewPoint.Projection);                    
                }

                mesh.Draw();
            }
        }

        private static void DrawModel(Effect effectOverride, Model model, Matrix world, IViewPoint viewpoint)
        {
            effectOverride.Parameters["World"].SetValue(world);
            effectOverride.Parameters["View"].SetValue(viewpoint.View);
            effectOverride.Parameters["Projection"].SetValue(viewpoint.Projection);            

            foreach (var mesh in model.Meshes)
            {
                var effects = new Effect[mesh.MeshParts.Count];

                for (var i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];

                    CopyTextures(part.Effect, effectOverride);
                    effects[i] = part.Effect;
                    part.Effect = effectOverride;

                }

                mesh.Draw();

                for (var i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];
                    part.Effect = effects[i];
                }
            }
        }

        // TODO: instead of copying all texture parameters we should extend the model type
        // and give it a bundle of textures per mesh part. So we can effectively set those on each shader
        // that needs them (and change the textures if need be).
        // or we could define multiple techniques for the same shader and do without overriding the texture!
        // (Making the shadowmap and colormap effects includes!)
        private static void CopyTextures(Effect source, Effect destination)
        {
            foreach (var parameter in source.Parameters)
            {
                if (parameter.ParameterType == EffectParameterType.Texture2D)
                {
                    destination.Parameters[parameter.Name]?.SetValue(parameter.GetValueTexture2D());
                }
            }
        }
    }
}
