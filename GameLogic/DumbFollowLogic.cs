using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic
{
    public sealed class DumbFollowLogic
    {
        private readonly CarDynamics CarDynamics;
        private readonly CarAnimation CarAnimation;

        private readonly AModel Target;
        private readonly Path Path;

        public DumbFollowLogic(AModel target, Path path, MetersPerSecond speed)
        {
            this.DistanceCovered = 0.0f;
            this.Target = target;
            this.CarAnimation = this.Target.Animation as CarAnimation;
            this.CarDynamics = new CarDynamics(new CarLayout(target));

            this.Path = path;
            this.Speed = speed;

            this.DistanceCovered = this.CarDynamics.AxleDistance;
            var back = this.Path.GetPositionAfter(0);
            var front = this.Path.GetPositionAfter(this.DistanceCovered);
            this.CarDynamics.BringAxlesInLine(front, back);
        }

        public MetersPerSecond Speed { get; set; }

        public float DistanceCovered { get; private set; }

        public void Update(Seconds elapsed)
        {
            if (this.DistanceCovered < this.Path.Length)
            {
                this.DistanceCovered = Math.Min(this.DistanceCovered + (elapsed * this.Speed), this.Path.Length);

                this.ComputeCarMovement();
                this.ComputeWheelMovement();
            }
        }

        private void ComputeCarMovement()
        {
            var newFrontAxlePosition = this.Path.GetPositionAfter(this.DistanceCovered);
            this.CarDynamics.BringAxlesInLine(newFrontAxlePosition);

            this.Target.Move(this.CarDynamics.GetCarSupportedCenter());

            var forward = this.CarDynamics.GetCarForward();
            if (forward.LengthSquared() > 0)
            {
                // TODO: why -Atan2?
                var yaw = -(float)Math.Atan2(forward.Z, forward.X);
                this.Target.Yaw = yaw;
            }
        }

        private void ComputeWheelMovement()
        {
            this.CarDynamics.UpdateWheelPositions();

            for (var i = 0; i < 4; i++)
            {
                var wheel = (WheelPosition)i;
                var rotation = this.CarDynamics.GetWheelRotationToCoverPositionChange(wheel);
                this.CarAnimation.WheelRoll[i] += rotation;
            }

            this.AngleFrontWheelsAlongPath();
        }

        private void AngleFrontWheelsAlongPath()
        {
            var frontAxle = this.CarDynamics.GetFrontAxlePosition();

            // TODO: find a better number for the wheel target
            // Maybe once we start following splines we can just take the diff betwen wheel positions?
            var wheelTarget = this.Path.GetPositionAfter(this.DistanceCovered + (this.CarDynamics.AxleDistance / 5));
            var axleToTarget = Vector3.Normalize(wheelTarget - frontAxle);

            if (axleToTarget.LengthSquared() > 0)
            {
                // TODO: why - Atan2?
                var angleToTarget = -(float)Math.Atan2(axleToTarget.Z, axleToTarget.X);

                var angleDifference = this.Target.Yaw - angleToTarget;
                var wheelYaw = -angleDifference;

                this.CarAnimation.WheelYaw[(int)WheelPosition.FrontLeft] = wheelYaw;
                this.CarAnimation.WheelYaw[(int)WheelPosition.FrontRight] = wheelYaw;
            }
        }
    }
}
