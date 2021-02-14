using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Particles
{
    public sealed class ParticleFountainComponent : AFountainComponent
    {
        public ParticleFountainComponent(Entity entity, GraphicsDevice device)
            : base(entity, device) { }
    }
}