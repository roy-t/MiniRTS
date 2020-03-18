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

        public DumbFollowLogic()
        {
            this.length = 0.0f;
            this.distanceCovered = 0.0f;
        }

        public void Start(AModel target, List<Vector2> path, MetersPerSecond speed)
        {
            this.target = target;
            this.car = new Car(target);
            this.path = path;
            this.speed = speed;
            this.distanceCovered = 0.0f;

            var length = 0.0f;
            for (var i = 1; i < this.path.Count; i++)
            {
                var from = this.GetPosition(i - 1);
                var to = this.GetPosition(i);

                var distance = Vector3.Distance(from, to);
                length += distance;
            }

            this.length = length;

            this.carAnimation = this.target.Animation as CarAnimation;
        }


        public void Update(Seconds elapsed)
        {
            // TODO: can we get rid of a lot of the minus signs here?
            if (this.distanceCovered >= this.length)
            {
                this.path.Reverse();
                this.distanceCovered = 0.0f;
            }

            this.distanceCovered += elapsed * this.speed;
            var position = this.GetPositionAfter(this.distanceCovered);
            var lookAt = this.GetLookAt(0.2f);

            this.target.Move(position);

            var n = Vector3.Normalize(lookAt - position);
            if (n.LengthSquared() > 0)
            {
                var yaw = -(float)Math.Atan2(n.Z, n.X);
                this.target.Yaw = yaw;
            }

            var frontAxle = this.car.GetFrontAxlePosition();

            var wheelTarget = this.GetLookAt(0.3f);
            var axleToTarget = Vector3.Normalize(wheelTarget - frontAxle);

            var angleToTarget = -(float)Math.Atan2(axleToTarget.Z, axleToTarget.X);

            var angleDifference = this.target.Yaw - angleToTarget;
            var wheelYaw = -angleDifference;

            this.carAnimation.FrontLeftWheelYaw = wheelYaw;
            this.carAnimation.FrontRightWheelYaw = wheelYaw;
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
