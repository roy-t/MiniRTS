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

        public PointLight Construct(Entity entity, Vector3 position, Color color, float radius, float intensity)
        {
            var light = new PointLight(entity, position, color, radius, intensity);
            this.Container.Add(light);

            return light;
        }
    }
}
