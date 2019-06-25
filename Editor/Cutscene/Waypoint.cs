using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
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

        [Icon(IconType.Waypoint)]
        [Editor(nameof(Position))]
        public Vector3 Position { get; }


        [Icon(IconType.LookAt)]
        [Editor(nameof(LookAt))]
        public Vector3 LookAt { get; }        
    }
}
