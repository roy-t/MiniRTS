using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic
{
    public sealed class DumbFollowLogic
    {
        private CarAnimation carAnimation;

        private Car car;
        private AModel target;
        private List<Vector2> path;
        private float length;

        private float distanceCovered;
        private MetersPerSecond speed;
        private float axleDistance;
        private Vector3 rearWheelPosition;
        private Vector3 frontWheelPosition;

        public DumbFollowLogic()
        {
            this.length = 0.0f;
            this.distanceCovered = 0.0f;
        }

        public void Start(AModel target, List<Vector2> path, MetersPerSecond speed)
        {
            this.target = target;
            this.carAnimation = this.target.Animation as CarAnimation;
            this.car = new Car(target);

            this.path = path;
            this.speed = speed;

            this.axleDistance = Vector3.Distance(this.car.GetRearAxlePosition(), this.car.GetFrontAxlePosition());
            this.distanceCovered = this.axleDistance;
            this.rearWheelPosition = this.GetPositionAfter(0);
            this.frontWheelPosition = this.GetPositionAfter(this.distanceCovered);

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


        public void Update(Seconds elapsed)
        {
            if (this.distanceCovered >= this.length)
            {
                this.path.Reverse();
                this.distanceCovered = 0.0f;
            }

            this.distanceCovered += elapsed * this.speed;

            this.frontWheelPosition = this.GetPositionAfter(this.distanceCovered);
            var normal = Vector3.Normalize(this.frontWheelPosition - this.rearWheelPosition);
            this.rearWheelPosition = this.frontWheelPosition - (normal * this.axleDistance);

            var position = Vector3.Lerp(this.frontWheelPosition, this.rearWheelPosition, 0.5f);

            this.target.Move(position);

            if (normal.LengthSquared() > 0)
            {
                // TODO: why -Atan2?
                var yaw = -(float)Math.Atan2(normal.Z, normal.X);
                this.target.Yaw = yaw;
            }

            this.AngleFrontWheelAlongPath();
        }

        private void AngleFrontWheelAlongPath()
        {
            // TODO: make wheel movement more realistic

            var frontAxle = this.car.GetFrontAxlePosition();

            var wheelTarget = this.GetLookAt(0.03f);
            var axleToTarget = Vector3.Normalize(wheelTarget - frontAxle);

            // TODO: why - Atan2?
            var angleToTarget = -(float)Math.Atan2(axleToTarget.Z, axleToTarget.X);

            var angleDifference = this.target.Yaw - angleToTarget;
            var wheelYaw = -angleDifference;

            this.carAnimation.FrontLeftWheelYaw = wheelYaw;
            this.carAnimation.FrontRightWheelYaw = wheelYaw;
        }

        private Vector3 FindPositionWithLengthTo(float searchMax, Vector3 measurePosition, float length)
        {
            var step = 0.01f;
            while (searchMax > 0)
            {
                searchMax -= step;
                var position = this.GetPositionAfter(searchMax);
                var distance = Vector3.Distance(measurePosition, position);
                if (distance >= length)
                {
                    return position;
                }
            }

            return this.GetPositionAfter(0);
        }

        public Vector3 GetLookAt(float lookAhead) => this.GetPositionAfter(this.distanceCovered + lookAhead);

        private Vector3 GetPositionAfter(float distanceCovered)
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
    }
}
