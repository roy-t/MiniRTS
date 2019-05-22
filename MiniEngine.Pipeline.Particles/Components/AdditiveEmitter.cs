using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Particles.Components
{
    public sealed class AdditiveEmitter : AEmitter
    {
        public AdditiveEmitter(Vector3 position, Texture2D texture, int rows, int columns, float scale)
            : base(position, texture, rows, columns, scale) { }
    }
}