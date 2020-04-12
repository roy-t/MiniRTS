using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
    // TODO: use Pose 
    public sealed class Waypoint : IComponent
    {
        public Waypoint(Entity entity, MetersPerSecond speed, Vector3 position, Vector3 lookAt)
        {
            this.Entity = entity;
            this.Speed = speed;
            this.Position = position;
            this.LookAt = lookAt;
        }

        public Entity Entity { get; }

        [Editor(nameof(Speed))]
        public MetersPerSecond Speed { get; }

        [Editor(nameof(Position))]
        public Vector3 Position { get; }

        [Editor(nameof(LookAt))]
        public Vector3 LookAt { get; }
    }
}
