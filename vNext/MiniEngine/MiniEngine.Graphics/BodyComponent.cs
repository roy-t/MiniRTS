using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics
{
    public sealed class BodyComponent : AComponent
    {
        public BodyComponent(Entity entity)
            : base(entity)
        {
            this.World = Matrix.Identity;
        }

        public Matrix World { get; }
    }
}
