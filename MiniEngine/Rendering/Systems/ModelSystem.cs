using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;

namespace MiniEngine.Rendering.Systems
{
    public sealed class ModelSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Dictionary<Entity, ModelPose> Models;

        public ModelSystem(GraphicsDevice device)
        {
            this.Device = device;
            this.Models = new Dictionary<Entity, ModelPose>();
        }

        public void Add(Entity entity, Model model, Matrix pose)
        {
            this.Models.Add(entity, new ModelPose(model, pose));
        }

        public void Remove(Entity entity)
        {
            this.Models.Remove(entity);
        }

        public void DrawModels(IViewPoint viewPoint)
        {

            foreach (var modelPose in this.Models.Values)
            {
                DrawModel(modelPose.Model, modelPose.Pose, viewPoint);
            }            
        }

        public void DrawModels(IViewPoint viewPoint, Effect effectOverride)
        {

            foreach (var modelPose in this.Models.Values)
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
    }
}
