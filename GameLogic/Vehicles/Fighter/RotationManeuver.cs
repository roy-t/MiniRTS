using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class RotationManeuver : IManeuver
    {
        private readonly Pose Pose;

        private readonly Vector3 LinearVelocity;

        private readonly Seconds YawDuration;
        private readonly Seconds PitchDuration;
        private readonly float Acceleration;

        private readonly float YawDirection;
        private float yawVelocity;

        private readonly float PitchDirection;
        private float pitchVelocity;

        private Seconds accumulator;

        public RotationManeuver(Pose pose, Vector3 currentVelocity, float targetYaw, Seconds yawDuration, float targetPitch, Seconds pitchDuration, float acceleration)
        {
            this.Pose = pose;

            this.LinearVelocity = currentVelocity;
            this.YawDuration = yawDuration;
            this.PitchDuration = pitchDuration;
            this.Acceleration = acceleration;

            this.YawDirection = Math.Sign(AngleMath.DistanceRadians(pose.Yaw, targetYaw));
            this.PitchDirection = Math.Sign(AngleMath.DistanceRadians(pose.Pitch, targetPitch));
        }

        public bool Completed { get; private set; }

        public void Initiate() { }

        public void Update(Seconds elapsed)
        {
            this.accumulator += elapsed;

            this.UpdateYaw(elapsed);
            this.UpdatePitch(elapsed);

            this.ApplyForces(elapsed);

            this.Completed =
                this.accumulator >= this.YawDuration &&
                this.accumulator >= this.PitchDuration;
        }

        private void ApplyForces(Seconds elapsed)
        {
            this.Pose.Move(this.Pose.Position + (this.LinearVelocity * elapsed));

            var yawChange = this.yawVelocity * elapsed;
            var yaw = this.Pose.Yaw + yawChange;

            var pitchChange = this.pitchVelocity * elapsed;
            var pitch = this.Pose.Pitch + pitchChange;
            this.Pose.Rotate(yaw, pitch, this.Pose.Roll);
        }

        private void UpdateYaw(Seconds elapsed)
        {
            if (this.accumulator < this.YawDuration / 2.0f)
            {
                this.yawVelocity += this.Acceleration * elapsed * this.YawDirection;
            }
            else if (this.accumulator < this.YawDuration)
            {
                this.yawVelocity -= this.Acceleration * elapsed * this.YawDirection; ;
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
