using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Basics.Components
{
    public sealed class Offset : IComponent
    {
        public Offset(Entity entity, Vector3 position, float yaw, float pitch, float roll, Entity target)
        {
            this.Entity = entity;
            this.Position = position;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;
            this.Target = target;
        }

        public Entity Entity { get; }

        [Editor(nameof(Position))]
        public Vector3 Position { get; set; }

        [Editor(nameof(Yaw))]
        public float Yaw { get; set; }

        [Editor(nameof(Pitch))]
        public float Pitch { get; set; }

        [Editor(nameof(Roll))]
        public float Roll { get; set; }
        public Entity Target { get; }
    }
}
