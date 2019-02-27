using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Particles.Factories
{
    public sealed class EmitterFactory : AComponentFactory<Emitter>
    {
        public EmitterFactory(GraphicsDevice device, EntityLinker linker)
            : base(device, linker) { }

        public void Construct(Entity entity, Vector3 position, Texture2D texture, int rows, int columns, float scale)
        {
            var emitter = new Emitter(position, texture, rows, columns, scale);
            this.Linker.AddComponent(entity, emitter);
        }
    }
}
