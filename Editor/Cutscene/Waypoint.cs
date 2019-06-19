using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
    public sealed class Waypoint : IComponent
    {
        public Waypoint(MetersPerSecond speed, Vector3 position, Vector3 lookAt)
        {
            this.Speed = speed;
            this.Position = position;
            this.LookAt = lookAt;
        }

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
