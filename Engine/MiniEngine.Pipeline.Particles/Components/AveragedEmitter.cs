using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Particles.Components
{
    public sealed class AveragedEmitter : AEmitter
    {
        public AveragedEmitter(Entity entity, Vector3 position, Texture2D texture, int rows, int columns, float scale)
            : base(entity, position, texture, rows, columns, scale) { }
    }
}