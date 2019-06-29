using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

using DirectionalLight = MiniEngine.Pipeline.Lights.Components.DirectionalLight;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class DirectionalLightFactory : AComponentFactory<DirectionalLight>
    {
        public DirectionalLightFactory(GraphicsDevice device, IComponentContainer<DirectionalLight> container)
            : base(device, container) { }

        public void Construct(Entity entity, Vector3 direction, Color color)
        {
            var light = new DirectionalLight(entity, direction, color);
            this.Container.Add(light);
        }
    }
}
