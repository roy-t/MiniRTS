using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics
{
    public sealed class TransformComponent : AComponent
    {
        public TransformComponent(Entity entity, Matrix matrix)
            : base(entity)
        {
            this.Matrix = matrix;
        }

        public TransformComponent(Entity entity)
            : base(entity)
        {
            this.Matrix = Matrix.Identity;
        }

        public Matrix Matrix { get; set; }
    }
}
