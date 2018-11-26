using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

using DirectionalLight = MiniEngine.Pipeline.Lights.Components.DirectionalLight;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class DirectionalLightFactory : AComponentFactory<DirectionalLight>
    {
        public DirectionalLightFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity, Vector3 direction, Color color)
        {
            var light = new DirectionalLight(direction, color);
            this.Linker.AddComponent(entity, light);
        }
    }
}
