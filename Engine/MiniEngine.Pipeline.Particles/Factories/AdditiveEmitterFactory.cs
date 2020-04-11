using Microsoft.Xna.Framework;
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

        public AdditiveEmitter ConstructAdditiveEmitter(Entity entity, Vector3 position, Texture2D texture, int rows, int columns, float scale)
        {
            var emitter = new AdditiveEmitter(entity, position, texture, rows, columns, scale);
            this.Container.Add(entity, emitter);

            return emitter;
        }
    }
}
