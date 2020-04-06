using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;
using Roy_T.AStar.Primitives;

namespace MiniEngine.GameLogic
{
    public sealed class PathFollowLogic
    {
        private readonly CarDynamics CarDynamics;
        private readonly CarAnimation CarAnimation;
        private readonly WorldGrid WorldGrid;
        private readonly AModel Target;
        private readonly Path Path;

        public GridPosition lastReserved;

        public PathFollowLogic(WorldGrid worldGrid, Car car, Path path, MetersPerSecond speed)
        {
            this.DistanceCovered = 0.0f;
            this.WorldGrid = worldGrid;
            this.Target = car.Model;
            this.CarAnimation = car.Animation;
            this.CarDynamics = car.Dynamics;// new CarDynamics(car.Layout);

            this.Path = path;
            this.Speed = speed;

            this.lastReserved = this.WorldGrid.ToGridPosition(this.CarDynamics.GetCarSupportedCenter());
        }

        public MetersPerSecond Speed { get; set; }

        public float DistanceCovered { get; private set; }

        public void Update(Seconds elapsed)
        {
            if (this.DistanceCovered < this.Path.Length)
            {
                var lookAhead = this.LookAhead(this.CarDynamics.AxleDistance / 5);
                var toReserve = this.WorldGrid.ToGridPosition(lookAhead);
                if (toReserve != this.lastReserved)
                {
                    if (this.WorldGrid.Reserve(this.Target.Entity, toReserve))
                    {
                        this.WorldGrid.Free(this.lastReserved);
                        this.lastReserved = toReserve;
                    }
                    else
                    {
                        return;
                    }
                }

                this.DistanceCovered = Math.Min(this.DistanceCovered + (elapsed * this.Speed), this.Path.Length);

                this.ComputeCarMovement();
                this.ComputeWheelMovement();
            }
        }

        private Vector3 LookAhead(float amount)
            => this.Path.GetPositionAfter(this.DistanceCovered + amount);

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
            var sum = 0.0f;
            for (var i = 0; i < 4; i++)
            {
                var wheel = (WheelPosition)i;
                var rotation = this.CarDynamics.GetWheelRotationToCoverPositionChange(wheel);
                this.CarAnimation.WheelRoll[i] += rotation;
                sum += rotation;
            }

            this.AngleFrontWheelsAlongPath(sum);
        }

        private void AngleFrontWheelsAlongPath(float sum)
        {
            var frontAxle = this.CarDynamics.GetProjectedFrontAxlePosition();

            var axleToTarget = Vector3.Zero;
            if (sum > 0)
            {
                var lookAhead = this.LookAhead(this.CarDynamics.AxleDistance / 5);
                axleToTarget = Vector3.Normalize(lookAhead - frontAxle);
            }
            else
            {
                var lookAhead = this.LookAhead(-this.CarDynamics.AxleDistance / 5);
                axleToTarget = Vector3.Normalize(lookAhead - frontAxle);
            }


            // TODO: find a better number for the wheel target
            // Maybe once we start following splines we can just take the diff betwen wheel positions?         

            if (axleToTarget.LengthSquared() > 0)
            {
                // TODO: why - Atan2?
                var angleToTarget = -(float)Math.Atan2(axleToTarget.Z, axleToTarget.X);

                var angleDifference = this.Target.Yaw - angleToTarget;
                //angleDifference = MathHelper.WrapAngle(angleDifference);

                //if (angleDifference > MathHelper.PiOver2)
                //{
                //    angleDifference -= MathHelper.Pi;
                //}

                //if (angleDifference < -MathHelper.PiOver2)
                //{
                //    angleDifference += MathHelper.Pi;
                //}



                var wheelYaw = -angleDifference;

                this.CarAnimation.WheelYaw[(int)WheelPosition.FrontLeft] = wheelYaw;
                this.CarAnimation.WheelYaw[(int)WheelPosition.FrontRight] = wheelYaw;
            }
        }
    }
}
