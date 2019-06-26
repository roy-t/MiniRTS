using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Projectors.Components;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Units;

namespace MiniEngine.Pipeline.Projectors.Systems
{
    public sealed class DynamicTextureSystem : IUpdatableSystem
    {
        private readonly GraphicsDevice Device;
        private readonly EntityLinker EntityLinker;

        private readonly IComponentContainer<DynamicTexture> Container;

        public DynamicTextureSystem(GraphicsDevice device, EntityLinker entityLinker, IComponentContainer<DynamicTexture> container)
        {
            this.Device = device;
            this.EntityLinker = entityLinker;
            this.Container = container;
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            for(var i = 0; i < this.Container.Count; i++)
            {
                var dynamicTexture = this.Container[i];

                var input = dynamicTexture.Input;
                input.Update(dynamicTexture.ViewPoint, elapsed, dynamicTexture.GBuffer, dynamicTexture.Pass);
                dynamicTexture.Pipeline.Execute(input, dynamicTexture.Label);
            }
        }
    }
}
