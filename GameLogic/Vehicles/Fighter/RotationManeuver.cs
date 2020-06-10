using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class RotationManeuver : IManeuver
    {
        private readonly Vector3 LinearVelocity;

        private readonly Seconds YawDuration;
        private readonly Seconds PitchDuration;
        private readonly float Acceleration;

        private readonly float YawDirection;
        private float yawVelocity;

        private readonly float PitchDirection;
        private float pitchVelocity;

        private Seconds accumulator;

        public RotationManeuver(Vector3 currentVelocity, float yawDirection, Seconds yawDuration, float pitchDirection, Seconds pitchDuration, float acceleration)
        {
            this.LinearVelocity = currentVelocity;
            this.YawDuration = yawDuration;
            this.PitchDuration = pitchDuration;
            this.Acceleration = acceleration;

            this.YawDirection = yawDirection;
            this.PitchDirection = pitchDirection;
        }

        public bool Completed { get; private set; }

        public void Update(Pose pose, Seconds elapsed)
        {
            this.accumulator += elapsed;

            this.UpdateYaw(elapsed);
            this.UpdatePitch(elapsed);

            this.ApplyForces(pose, elapsed);

            this.Completed =
                this.accumulator >= this.YawDuration &&
                this.accumulator >= this.PitchDuration;
        }

        private void ApplyForces(Pose pose, Seconds elapsed)
        {
            pose.Move(pose.Position + (this.LinearVelocity * elapsed));

            var yawChange = this.yawVelocity * elapsed;
            var yaw = pose.Yaw + yawChange;

            var pitchChange = this.pitchVelocity * elapsed;
            var pitch = pose.Pitch + pitchChange;
            pose.Rotate(yaw, pitch, pose.Roll);
        }

        private void UpdateYaw(Seconds elapsed)
        {
            if (this.accumulator < this.YawDuration / 2.0f)
            {
                this.yawVelocity += this.Acceleration * elapsed * this.YawDirection;
            }
            else if (this.accumulator < this.YawDuration)
            {
                this.yawVelocity -= this.Acceleration * elapsed * this.YawDirection;
            }
        }

        private void UpdatePitch(Seconds elapsed)
        {
            if (this.accumulator < this.PitchDuration / 2.0f)
            {
                this.pitchVelocity += this.Acceleration * elapsed * this.PitchDirection;
            }
            else if (this.accumulator < this.PitchDuration)
            {
                this.pitchVelocity -= this.Acceleration * elapsed * this.PitchDirection;
            }
        }
    }
}
