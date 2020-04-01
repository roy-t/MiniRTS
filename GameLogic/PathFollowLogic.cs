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
        private Vector3 lookAhead;

        public PathFollowLogic(WorldGrid worldGrid, AModel target, CarAnimation carAnimation, Path path, MetersPerSecond speed)
        {
            this.DistanceCovered = 0.0f;
            this.WorldGrid = worldGrid;
            this.Target = target;
            this.CarAnimation = carAnimation;
            this.CarDynamics = new CarDynamics(new CarLayout(target));

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
                this.LookAhead();

                var toReserve = this.WorldGrid.ToGridPosition(this.lookAhead);
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

        private void LookAhead()
            => this.lookAhead = this.Path.GetPositionAfter(this.DistanceCovered + (this.CarDynamics.AxleDistance / 5));

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
            var frontAxle = this.CarDynamics.GetProjectedFrontAxlePosition();

            // TODO: find a better number for the wheel target
            // Maybe once we start following splines we can just take the diff betwen wheel positions?            
            var axleToTarget = Vector3.Normalize(this.lookAhead - frontAxle);

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
