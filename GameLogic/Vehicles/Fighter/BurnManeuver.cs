using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class BurnManeuver : IManeuver
    {
        private readonly Vector3 Direction;
        private readonly float Acceleration;
        private readonly Seconds Duration;

        private Vector3 velocity;
        private Seconds accumulator;

        public BurnManeuver(Vector3 direction, Vector3 currentVelocity, float acceleration, Seconds duration)
        {
            this.Direction = direction;
            this.velocity = currentVelocity;
            this.Acceleration = acceleration;
            this.Duration = duration;

            this.accumulator = 0.0f;
        }

        public bool Completed { get; private set; }

        public void Update(Pose pose, Seconds elapsed)
        {
            this.accumulator += elapsed;
            if (this.accumulator < this.Duration)
            {
                this.velocity += this.Direction * this.Acceleration * elapsed;
                pose.Move(pose.Position + (this.velocity * elapsed));
            }
            else
            {
                this.Completed = true;
            }
        }
    }
}
