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

        public AveragedEmitter ConstructAveragedEmitter(Entity entity, Texture2D texture, int rows, int columns)
        {
            var emitter = new AveragedEmitter(entity, texture, rows, columns);
            this.Container.Add(emitter);

            return emitter;
        }
    }
}
