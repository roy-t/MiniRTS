using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class AmbientLightFactory : AComponentFactory<AmbientLight>
    {
        public AmbientLightFactory(GraphicsDevice device, IComponentContainer<AmbientLight> container)
            : base(device, container) { }

        public AmbientLight Construct(Entity entity, Color color)
        {
            var light = new AmbientLight(entity, color);
            this.Container.Add(light);

            return light;
        }
    }
}
