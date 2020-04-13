using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class PointLightFactory : AComponentFactory<PointLight>
    {
        public PointLightFactory(GraphicsDevice device, IComponentContainer<PointLight> container)
            : base(device, container) { }

        public PointLight Construct(Entity entity, Color color, float radius, float intensity)
        {
            var light = new PointLight(entity, color, radius, intensity);
            this.Container.Add(entity, light);

            return light;
        }
    }
}
