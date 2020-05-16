using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class Accelerometer
    {
        private readonly Pose Pose;
        private Vector3 position;

        public Accelerometer(Pose pose)
        {
            this.Pose = pose;
        }

        public Vector3 Acceleration { get; private set; }
        public Vector3 Velocity { get; set; }

        public void Update(Seconds elapsed)
        {
            var newPosition = this.Pose.Position;
            var newVelocity = (newPosition - this.position) / elapsed;
            this.Acceleration = newVelocity - this.Velocity;

            this.Velocity = newVelocity;
            this.position = newPosition;
        }
    }
}
