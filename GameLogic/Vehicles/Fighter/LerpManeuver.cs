using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class LerpManeuver : IManeuver
    {
        private readonly Seconds Duration;

        private Seconds accumulator;

        private Vector3 startPosition;
        private float startYaw;
        private float startPitch;

        public LerpManeuver(Vector3 targetPosition, float targetYaw, float targetPitch, Seconds duration)
        {
            this.TargetPosition = targetPosition;
            this.TargetYaw = targetYaw;
            this.TargetPitch = targetPitch;
            this.Duration = duration;
        }

        public bool Completed { get; private set; }

        public Vector3 TargetPosition { get; }
        public float TargetYaw { get; }
        public float TargetPitch { get; }

        public void Update(Pose pose, Seconds elapsed)
        {
            if (this.accumulator == 0)
            {
                this.Initiate(pose);
            }

            this.accumulator += elapsed;
            var progress = this.accumulator / this.Duration;
            pose.Rotate(
                AngleMath.LerpRadians(this.startYaw, this.TargetYaw, progress),
                AngleMath.LerpRadians(this.startPitch, this.TargetPitch, progress),
                pose.Roll);

            pose.Move(Vector3.Lerp(this.startPosition, this.TargetPosition, progress));

            if (this.accumulator >= this.Duration)
            {
                this.Completed = true;
            }
        }

        private void Initiate(Pose pose)
        {
            this.startPosition = pose.Position;
            this.startYaw = pose.Yaw;
            this.startPitch = pose.Pitch;
        }
    }
}
