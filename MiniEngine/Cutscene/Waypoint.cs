using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;
using MiniEngine.Units;

namespace MiniEngine.CutScene
{
    public sealed class Waypoint : IComponent
    {
        public Waypoint(MetersPerSecond speed, Vector3 position)
        {
            this.Speed = speed;
            this.Position = position;
        }

        [Editor(nameof(Speed))]
        public MetersPerSecond Speed { get; }

        [Icon(IconType.LookAt)]
        [Editor(nameof(Position))]
        public Vector3 Position { get; }        
    }
}
