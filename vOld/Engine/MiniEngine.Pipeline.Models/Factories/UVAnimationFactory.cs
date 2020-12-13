using System;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class UVAnimationFactory : AComponentFactory<UVAnimation>
    {
        private readonly IComponentContainer<OpaqueModel> Models;

        public UVAnimationFactory(GraphicsDevice device, IComponentContainer<UVAnimation> container, IComponentContainer<OpaqueModel> models)
            : base(device, container)
        {
            this.Models = models;
        }

        public UVAnimation Construct(Entity entity, params string[] meshNames)
        {
            var model = this.Models.Get(entity);

            var indexOffset = new IndexOffset[meshNames.Length];

            for (var i = 0; i < meshNames.Length; i++)
            {
                var name = meshNames[i];
                var index = GetMeshIndex(model.Model, name);

                indexOffset[i] = new IndexOffset(name, index);
            }

            var animation = new UVAnimation(entity, indexOffset);
            this.Container.Add(animation);

            return animation;
        }


        private int GetMeshIndex(Model model, string name)
        {
            for (var i = 0; i < model.Meshes.Count; i++)
            {
                var mesh = model.Meshes[i];
                if (mesh.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            throw new Exception($"Could not find mesh with name {name} in model");
        }
    }
}
