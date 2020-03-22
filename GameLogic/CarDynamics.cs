using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.GameLogic
{
    public sealed class CarDynamics
    {
        private const int FrontAxleIndex = 0;
        private const int RearAxleIndex = 1;
        private const int Axles = 2;
        private const int Wheels = 4;

        private readonly CarLayout Car;
        private readonly Vector3[] LastWheelPositions;
        private readonly Vector3[] CurrentWheelPositions;
        private readonly Vector3[] AxlePositions;

        private readonly float[] WheelCircumferences;

        public CarDynamics(CarLayout car)
        {
            this.Car = car;
            this.LastWheelPositions = new Vector3[Wheels];
            this.CurrentWheelPositions = new Vector3[Wheels];
            this.WheelCircumferences = this.ComputeWheelCircumferences();

            this.AxlePositions = new Vector3[Axles];

            this.UpdateWheelPositions();
            this.PushWheelPositions();

            this.ComputeAxlePositions();
            this.AxleDistance = Vector3.Distance(car.GetFrontAxlePosition(), car.GetRearAxlePosition());
        }

        public float AxleDistance { get; }

        public void UpdateWheelPositions()
        {
            this.PushWheelPositions();
            for (var i = 0; i < Wheels; i++)
            {
                this.CurrentWheelPositions[i] = this.Car.GetWheelPosition((WheelPosition)i);
            }
        }

        public float GetWheelRotationToCoverPositionChange(WheelPosition wheel)
        {
            var index = (int)wheel;

            var before = this.LastWheelPositions[index];
            var after = this.CurrentWheelPositions[index];
            var distance = Vector3.Distance(before, after);
            var circumference = this.WheelCircumferences[index];

            return (distance / circumference) * MathHelper.TwoPi;
        }

        public void BringAxlesInLine(Vector3 newFrontAxlePosition)
        {
            var rearAxlePosition = this.AxlePositions[RearAxleIndex];
            this.BringAxlesInLine(newFrontAxlePosition, rearAxlePosition);
        }

        public void BringAxlesInLine(Vector3 newFrontAxlePosition, Vector3 newRearAxlePosition)
        {
            this.AxlePositions[FrontAxleIndex] = newFrontAxlePosition;
            var normal = Vector3.Normalize(newFrontAxlePosition - newRearAxlePosition);

            if (normal.LengthSquared() > 0)
            {
                this.AxlePositions[RearAxleIndex] = newFrontAxlePosition - (normal * this.AxleDistance);
            }
        }

        public Vector3 GetCarForward()
        {
            var frontAxlePosition = this.AxlePositions[FrontAxleIndex];
            var rearAxlePosition = this.AxlePositions[RearAxleIndex];
            return Vector3.Normalize(frontAxlePosition - rearAxlePosition);
        }

        public Vector3 GetCarSupportedCenter()
            => Vector3.Lerp(this.AxlePositions[FrontAxleIndex], this.AxlePositions[RearAxleIndex], 0.5f);

        public Vector3 GetFrontAxlePosition() => this.AxlePositions[FrontAxleIndex];


        public void ComputeAxlePositions()
        {

            this.AxlePositions[FrontAxleIndex] = this.Car.GetFrontAxlePosition();
            this.AxlePositions[RearAxleIndex] = this.Car.GetRearAxlePosition();
        }

        private float[] ComputeWheelCircumferences()
        {
            var circumferences = new float[Wheels];
            for (var i = 0; i < Wheels; i++)
            {
                var radius = this.Car.GetWheelRadius((WheelPosition)i);
                circumferences[i] = MathHelper.TwoPi * radius;
            }

            return circumferences;
        }

        private void PushWheelPositions() => Array.Copy(this.CurrentWheelPositions, 0, this.LastWheelPositions, 0, Wheels);
    }
}
