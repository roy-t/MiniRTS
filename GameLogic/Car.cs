using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;

namespace MiniEngine.GameLogic
{
    public sealed class Car : IComponent
    {
        public Car(OpaqueModel model, Pose pose, CarAnimation animation)
        {
            this.Entity = model.Entity;
            this.Model = model;
            this.Pose = pose;
            this.Animation = animation;

            this.Layout = new CarLayout(model, pose);
            this.Dynamics = new CarDynamics(this.Layout);

            this.Pose.SetOrigin(this.Dynamics.GetCarSupportedCenter());
        }

        public Entity Entity { get; }

        public AModel Model { get; }
        public Pose Pose { get; }
        public CarAnimation Animation { get; }
        public CarLayout Layout { get; }
        public CarDynamics Dynamics { get; }

        public void MoveAndTurn(Vector3 position, float yaw)
        {
            this.Pose.Move(position);
            this.Pose.Rotate(yaw, this.Pose.Pitch, this.Pose.Roll);

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

        public Vector3 Position => this.Pose.Position;

        public float Yaw => this.Pose.Yaw;

        public float Pitch => this.Pose.Pitch;

        public float Roll => this.Pose.Roll;
    }
}
