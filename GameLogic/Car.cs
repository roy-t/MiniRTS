using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.GameLogic
{
    public sealed class Car : IComponent
    {
        public Car(OpaqueModel model, CarAnimation animation)
        {
            this.Entity = model.Entity;
            this.Model = model;
            this.Animation = animation;

            this.Layout = new CarLayout(model);
            this.Dynamics = new CarDynamics(this.Layout);

            this.Model.Origin = this.Dynamics.GetCarSupportedCenter();
        }

        public Entity Entity { get; }

        public AModel Model { get; }
        public CarAnimation Animation { get; }
        public CarLayout Layout { get; }
        public CarDynamics Dynamics { get; }

        public void MoveAndTurn(Vector3 position, float yaw)
        {
            this.Model.Move(position);
            this.Model.Yaw = yaw;

            this.Dynamics.UpdateWheelPositions();
            this.Dynamics.ComputeProjectedAxlePositions();

        }

        public void AngleFrontWheels(float yaw)
        {
            this.Animation.WheelYaw[(int)WheelPosition.FrontLeft] = yaw;
            this.Animation.WheelYaw[(int)WheelPosition.FrontRight] = yaw;
        }

        public void RotateWheel(WheelPosition wheel, float rotationChange)
            => this.Animation.WheelRoll[(int)wheel] += rotationChange;

        // Derived properties

        public Vector3 Position => this.Model.Position;

        public float Yaw => this.Model.Yaw;

        public float Pitch => this.Model.Pitch;

        public float Roll => this.Model.Roll;
    }
}
