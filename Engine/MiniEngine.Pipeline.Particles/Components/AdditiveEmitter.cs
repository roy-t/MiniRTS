using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Particles.Components
{
    public sealed class AdditiveEmitter : AEmitter
    {
        public AdditiveEmitter(Entity entity, Texture2D texture, int rows, int columns)
            : base(entity, texture, rows, columns) { }
    }
}