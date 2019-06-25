using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Particles.Factories
{
    public sealed class AveragedEmitterFactory : AComponentFactory<AveragedEmitter>
    {
        public AveragedEmitterFactory(GraphicsDevice device, EntityLinker linker)
            : base(device, linker) { }

        public AveragedEmitter ConstructAveragedEmitter(Entity entity, Vector3 position, Texture2D texture, int rows, int columns, float scale)
        {
            var emitter = new AveragedEmitter(entity, position, texture, rows, columns, scale);
            this.Linker.AddComponent(entity, emitter);

            return emitter;
        }
    }
}
