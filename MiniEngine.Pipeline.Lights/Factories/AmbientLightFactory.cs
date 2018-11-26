using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Lights.Factories
{
    public sealed class AmbientLightFactory : AComponentFactory<AmbientLight>
    {
        public AmbientLightFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity, Color color)
        {
            var light = new AmbientLight(color);
            this.Linker.AddComponent(entity, light);
        }
    }
}
