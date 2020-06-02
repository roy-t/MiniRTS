using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class BurnRetroBurnManeuver : IManeuver
    {
        private SubManeuver subManeuver;

        private Pose pose;
        private readonly Vector3 targetPosition;
        private float maxLinearAcceleration;
        private float maxAngularAcceleration;

        private Vector3 direction;
        private float linearVelocity;
        private float angularYawVelocity;
        private float angularPitchVelocity;

        private float burnDuration;
        private float yawRotationDuration;
        private float pitchDirection;
        private float pitchRotationDuration;

        private float targetYaw;
        private float targetPitch;

        public BurnRetroBurnManeuver(Pose pose, Vector3 targetPosition, float maxLinearAcceleration, float maxAngularAcceleration)
        {
            this.pose = pose;
            this.targetPosition = targetPosition;
            this.maxLinearAcceleration = maxLinearAcceleration;
            this.maxAngularAcceleration = maxAngularAcceleration;

            this.direction = pose.GetForward();
            var startPitch = AngleMath.PitchFromVector(this.direction);
            this.targetYaw = AngleMath.YawFromVector(-this.direction);
            this.targetPitch = AngleMath.PitchFromVector(-this.direction);

            var distance = Vector3.Distance(targetPosition, this.pose.Position);
            this.yawRotationDuration = ComputeRotationTime(MathHelper.Pi, maxAngularAcceleration);

            var pitchDistance = AngleMath.DistanceRadians(startPitch, this.targetPitch);
            this.pitchDirection = startPitch > this.targetPitch ? -1 : 1;
            this.pitchRotationDuration = ComputeRotationTime(pitchDistance, maxAngularAcceleration);
            this.burnDuration = ComputeBurnTime(distance, maxLinearAcceleration, this.yawRotationDuration);

            this.subManeuver = SubManeuver.ProgradeBurn;
        }

        public bool Completed { get; private set; }

        public void Update(Seconds elapsed)
        {
            switch (this.subManeuver)
            {
                case SubManeuver.ProgradeBurn:
                    this.PerformProgradeBurn(elapsed);
                    this.ApplyForces(elapsed);
                    break;
                case SubManeuver.Rotate:
                    this.ChangeYaw(elapsed);
                    this.ChangePitch(elapsed);
                    this.ApplyForces(elapsed);
                    break;
                case SubManeuver.RetrogradeBurn:
                    this.PerformRetroGradeBurn(elapsed);
                    this.ApplyForces(elapsed);
                    break;
                case SubManeuver.Adjust:
                    this.Adjust(elapsed);
                    break;
                default:
                    this.Completed = true;
                    break;
            }
        }

        private void ApplyForces(Seconds elapsed)
        {
            var change = this.linearVelocity * elapsed;
            this.pose.Move(this.pose.Position + (this.direction * change));

            var yawChange = this.angularYawVelocity * elapsed;
            var yaw = this.pose.Yaw + yawChange;

            var pitchChange = this.angularPitchVelocity * elapsed;
            var pitch = this.pose.Pitch + pitchChange;
            this.pose.Rotate(yaw, pitch, this.pose.Roll);
        }

        float accum5;
        private void Adjust(Seconds elapsed)
        {
            this.accum5 += elapsed;

            if (this.accum5 >= 1.0f)
            {
                this.subManeuver = SubManeuver.Completed;
            }
            else
            {
                var progress = elapsed;
                this.pose.Move(Vector3.Lerp(this.pose.Position, this.targetPosition, progress));
                this.pose.Rotate(
                    AngleMath.LerpRadians(this.pose.Yaw, this.targetYaw, progress),
                    AngleMath.LerpRadians(this.pose.Pitch, this.targetPitch, progress),
                    this.pose.Roll);
            }
        }

        float accum1;
        private void PerformProgradeBurn(Seconds elapsed)
        {
            this.accum1 += elapsed;
            if (this.accum1 >= this.burnDuration)
            {
                this.subManeuver = SubManeuver.Rotate;
            }
            else
            {
                this.linearVelocity += this.maxLinearAcceleration * elapsed;
            }
        }

        float accum2;
        private void ChangeYaw(Seconds elapsed)
        {
            this.accum2 += elapsed;

            if (this.accum2 < this.yawRotationDuration / 2.0f)
            {
                this.angularYawVelocity += this.maxAngularAcceleration * elapsed;
            }
            else if (this.accum2 < this.yawRotationDuration)
            {
                this.angularYawVelocity -= this.maxAngularAcceleration * elapsed;
            }
            else
            {
                this.subManeuver = SubManeuver.RetrogradeBurn;
            }
        }

        float accum4;
        private void ChangePitch(Seconds elapsed)
        {
            this.accum4 += elapsed;

            if (this.accum4 < this.pitchRotationDuration / 2.0f)
            {
                this.angularPitchVelocity += this.maxAngularAcceleration * elapsed * this.pitchDirection;
            }
            else if (this.accum4 < this.pitchRotationDuration)
            {
                this.angularPitchVelocity -= this.maxAngularAcceleration * elapsed * this.pitchDirection;
            }
        }

        float accum3;
        private void PerformRetroGradeBurn(Seconds elapsed)
        {
            this.accum3 += elapsed;
            if (this.accum3 >= this.burnDuration)
            {
                this.subManeuver = SubManeuver.Adjust;
            }
            else
            {
                this.linearVelocity -= this.maxLinearAcceleration * elapsed;
            }
        }

        private enum SubManeuver
        {
            ProgradeBurn,
            Rotate,
            RetrogradeBurn,
            Adjust,
            Completed
        }

        private static float ComputeBurnTime(double distance, double acceleration, double rotationTime)
        {
            var a = acceleration;
            var b = acceleration * rotationTime;
            var c = -distance;

            // solve ax^2 + bx + c = 0
            // d = b^2 -4ac
            var d = (b * b) - (4 * a * c);

            return (float)((-b + Math.Sqrt(d)) / (2 * a));
        }

        private static float ComputeRotationTime(double distance, double acceleration)
        {
            var a = acceleration;
            var b = 0;
            var c = -distance;

            var d = (b * b) - (4 * a * c);

            return (float)((-b + Math.Sqrt(d)) / (2 * a)) * 2;
        }
    }
}