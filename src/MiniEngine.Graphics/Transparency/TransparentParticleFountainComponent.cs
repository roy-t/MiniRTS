using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Particles;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Transparency
{
    public sealed class TransparentParticleFountainComponent : AFountainComponent
    {
        public TransparentParticleFountainComponent(Entity entity, GraphicsDevice device)
            : base(entity, device) { }
    }
}
