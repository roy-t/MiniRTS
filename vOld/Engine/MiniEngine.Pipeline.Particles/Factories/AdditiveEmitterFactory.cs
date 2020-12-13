using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Particles.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Particles.Factories
{
    public sealed class AdditiveEmitterFactory : AComponentFactory<AdditiveEmitter>
    {
        public AdditiveEmitterFactory(GraphicsDevice device, IComponentContainer<AdditiveEmitter> container)
            : base(device, container) { }

        public AdditiveEmitter ConstructAdditiveEmitter(Entity entity, Texture2D texture, int rows, int columns)
        {
            var emitter = new AdditiveEmitter(entity, texture, rows, columns);
            this.Container.Add(emitter);

            return emitter;
        }
    }
}
