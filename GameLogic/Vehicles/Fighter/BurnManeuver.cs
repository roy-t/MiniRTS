using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class BurnManeuver : IManeuver
    {
        private readonly Pose Pose;
        private readonly float Acceleration;
        private readonly Seconds Duration;

        private Vector3 velocity;
        private Seconds accumulator;

        public BurnManeuver(Pose pose, Vector3 linearVelocity, float acceleration, Seconds duration)
        {
            this.Pose = pose;

            this.velocity = linearVelocity;
            this.Acceleration = acceleration;
            this.Duration = duration;

            this.accumulator = 0.0f;
        }

        public bool Completed { get; private set; }

        public void Initiate() { }

        public void Update(Seconds elapsed)
        {
            this.accumulator += elapsed;
            if (this.accumulator < this.Duration)
            {
                var forward = this.Pose.GetForward();
                this.velocity += forward * this.Acceleration * elapsed;
                this.Pose.Move(this.Pose.Position + (this.velocity * elapsed));
            }
            else
            {
                this.Completed = true;
            }
        }
    }
}
