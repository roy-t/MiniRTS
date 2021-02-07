using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Particles;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Transparency
{
    public sealed class TransparentParticleEmitterComponent : ParticleEmitterComponent
    {
        public TransparentParticleEmitterComponent(Entity entity, IParticleSpawnFunction spawnFunction, IParticleUpdateFunction updateFunction, GraphicsDevice device, Texture2D texture)
            : base(entity, spawnFunction, updateFunction, device, texture) { }
    }
}
