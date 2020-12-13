using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Particles.Components
{
    public sealed class AveragedEmitter : AEmitter
    {
        public AveragedEmitter(Entity entity, Texture2D texture, int rows, int columns)
            : base(entity, texture, rows, columns) { }
    }
}