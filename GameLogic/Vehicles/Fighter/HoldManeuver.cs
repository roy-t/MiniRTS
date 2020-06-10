using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class HoldManeuver : IManeuver
    {
        private readonly Pose Pose;
        private readonly Vector3 TargetPosition;
        private readonly float TargetYaw;
        private readonly float TargetPitch;
        private readonly Seconds Duration;

        private Seconds accumulator;

        private Vector3 startPosition;
        private float startYaw;
        private float startPitch;

        public HoldManeuver(Pose pose, Vector3 targetPosition, float targetYaw, float targetPitch, Seconds duration)
        {
            this.Pose = pose;
            this.TargetPosition = targetPosition;
            this.TargetYaw = targetYaw;
            this.TargetPitch = targetPitch;
            this.Duration = duration;
        }

        public bool Completed { get; private set; }

        public void Initiate()
        {
            this.startPosition = this.Pose.Position;
            this.startYaw = this.Pose.Yaw;
            this.startPitch = this.Pose.Pitch;
        }

        public void Update(Seconds elapsed)
        {
            this.accumulator += elapsed;
            var progress = this.accumulator / this.Duration;
            this.Pose.Rotate(
                AngleMath.LerpRadians(this.startYaw, this.TargetYaw, progress),
                AngleMath.LerpRadians(this.startPitch, this.TargetPitch, progress),
                this.Pose.Roll);

            this.Pose.Move(Vector3.Lerp(this.startPosition, this.TargetPosition, progress));

            if (this.accumulator >= this.Duration)
            {
                this.Completed = true;
            }
        }
    }
}
