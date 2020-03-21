using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic
{
    // TODO: move all path related methods to a Path class
    public sealed class DumbFollowLogic
    {
        private CarDynamics carDynamics;
        private CarAnimation carAnimation;

        private AModel target;
        private List<Vector2> path;
        private float length;

        public DumbFollowLogic()
        {
            this.length = 0.0f;
            this.DistanceCovered = 0.0f;
        }

        public void Start(AModel target, List<Vector2> path, MetersPerSecond speed)
        {
            this.target = target;
            this.carAnimation = this.target.Animation as CarAnimation;
            this.carDynamics = new CarDynamics(new CarLayout(target));

            this.path = path;
            this.Speed = speed;

            this.DistanceCovered = this.carDynamics.AxleDistance;
            var back = this.GetPositionAfter(0);
            var front = this.GetPositionAfter(this.DistanceCovered);
            this.carDynamics.BringAxlesInLine(front, back);

            this.ComputePathLength();
        }

        public MetersPerSecond Speed { get; set; }

        public float DistanceCovered { get; private set; }

        public void Update(Seconds elapsed)
        {
            if (this.DistanceCovered < this.length)
            {
                this.DistanceCovered = Math.Min(this.DistanceCovered + (elapsed * this.Speed), this.length);

                this.ComputeCarMovement();
                this.ComputeWheelMovement();
            }
        }

        private void ComputeCarMovement()
        {
            var newFrontAxlePosition = this.GetPositionAfter(this.DistanceCovered);
            this.carDynamics.BringAxlesInLine(newFrontAxlePosition);

            this.target.Move(this.carDynamics.GetCarSupportedCenter());

            var forward = this.carDynamics.GetCarForward();
            if (forward.LengthSquared() > 0)
            {
                // TODO: why -Atan2?
                var yaw = -(float)Math.Atan2(forward.Z, forward.X);
                this.target.Yaw = yaw;
            }
        }

        private void ComputeWheelMovement()
        {
            this.carDynamics.UpdateWheelPositions();

            for (var i = 0; i < 4; i++)
            {
                var wheel = (WheelPosition)i;
                var rotation = this.carDynamics.GetWheelRotationToCoverPositionChange(wheel);
                this.carAnimation.WheelRoll[i] += rotation;
            }

            this.AngleFrontWheelsAlongPath();
        }

        private void AngleFrontWheelsAlongPath()
        {
            var frontAxle = this.carDynamics.GetFrontAxlePosition();

            // TODO: find a better number for the wheel target
            // Maybe once we start following splines we can just take the diff betwen wheel positions?
            var wheelTarget = this.GetPositionAfter(this.DistanceCovered + (this.carDynamics.AxleDistance / 5));
            var axleToTarget = Vector3.Normalize(wheelTarget - frontAxle);

            // TODO: why - Atan2?
            var angleToTarget = -(float)Math.Atan2(axleToTarget.Z, axleToTarget.X);

            var angleDifference = this.target.Yaw - angleToTarget;
            var wheelYaw = -angleDifference;

            this.carAnimation.WheelYaw[(int)WheelPosition.FrontLeft] = wheelYaw;
            this.carAnimation.WheelYaw[(int)WheelPosition.FrontRight] = wheelYaw;
        }

        public Vector3 GetPositionAfter(float distanceCovered)
        {
            var toCover = distanceCovered;
            for (var i = 1; i < this.path.Count; i++)
            {
                var from = this.GetPosition(i - 1);
                var to = this.GetPosition(i);

                var distance = Vector3.Distance(from, to);
                if (toCover > distance)
                {
                    toCover -= distance; ;
                }
                else
                {
                    return Vector3.Lerp(from, to, toCover / distance);
                }
            }

            return this.GetPosition(this.path.Count - 1);
        }

        private Vector3 GetPosition(int index)
        {
            var v2 = this.path[index];
            return new Vector3(v2.X, 0, v2.Y);
        }

        private void ComputePathLength()
        {
            var length = 0.0f;
            for (var i = 1; i < this.path.Count; i++)
            {
                var from = this.GetPosition(i - 1);
                var to = this.GetPosition(i);

                var distance = Vector3.Distance(from, to);
                length += distance;
            }

            this.length = length;
        }
    }
}
