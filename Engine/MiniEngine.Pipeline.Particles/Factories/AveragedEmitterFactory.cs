using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Particles.Factories
{
    public sealed class AveragedEmitterFactory : AComponentFactory<AveragedEmitter>
    {
        public AveragedEmitterFactory(GraphicsDevice device, IComponentContainer<AveragedEmitter> container)
            : base(device, container) { }

        public AveragedEmitter ConstructAveragedEmitter(Entity entity, Vector3 position, Texture2D texture, int rows, int columns, float scale)
        {
            var emitter = new AveragedEmitter(entity, position, texture, rows, columns, scale);
            this.Container.Add(entity, emitter);

            return emitter;
        }
    }
}
