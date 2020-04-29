using System;
using Microsoft.Xna.Framework;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles
{
    public sealed class TankPathFollowLogic
    {
        private readonly WorldGrid Grid;
        private readonly Tank Tank;
        private readonly Path Path;
        private readonly MetersPerSecond MaxSpeed;

        private float distanceCovered;

        public TankPathFollowLogic(WorldGrid grid, Tank tank, Path path, MetersPerSecond maxSpeed)
        {
            this.Grid = grid;
            this.Tank = tank;
            this.Path = path;
            this.MaxSpeed = maxSpeed;

            this.distanceCovered = 0.0f;
        }

        public void Update(Seconds elapsed)
        {
            if (this.distanceCovered < this.Path.Length)
            {
                this.distanceCovered = Math.Min(this.distanceCovered + (elapsed * this.MaxSpeed), this.Path.Length);
                this.ComputeTankMovement();
            }
        }

        private void ComputeTankMovement()
        {
            var newPosition = this.Path.GetPositionAfter(this.distanceCovered);
            var lookAheadPosition = this.Path.GetPositionAfter(this.distanceCovered + 0.1f);
            var direction = lookAheadPosition - newPosition;

            if (direction.LengthSquared() > 0)
            {
                var normalizedDirection = Vector3.Normalize(direction);
                var yaw = (float)Math.Atan2(normalizedDirection.X, normalizedDirection.Z) + MathHelper.Pi;

                this.Tank.MoveAndTurn(newPosition, yaw);
            }
        }
    }
}
