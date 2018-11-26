using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class PointLightFactory : AComponentFactory<PointLight>
    {
        public PointLightFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity, Vector3 position, Color color, float radius, float intensity)
        {
            var light = new PointLight(position, color, radius, intensity);
            this.Linker.AddComponent(entity, light);
        }
    }
}
