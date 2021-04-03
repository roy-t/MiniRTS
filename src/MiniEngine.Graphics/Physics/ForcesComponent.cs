using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Physics
{
    public sealed class ForcesComponent : AComponent
    {
        public ForcesComponent(Entity entity)
            : base(entity) { }

        public Vector3 LastPosition { get; set; }
        public Vector3 LastVelocity { get; set; }

        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
    }
}
