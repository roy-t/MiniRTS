using System;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public class RotationManeuver : IManeuver
    {
        private readonly Pose Pose;
        private Seconds accumulator;

        public RotationManeuver(Pose pose, float targetYaw, float targetPitch, float radiansPerSecond)
        {
            this.Pose = pose;
            this.StartYaw = pose.Yaw;
            this.TargetYaw = targetYaw;
            this.StartPich = pose.Pitch;
            this.TargetPitch = targetPitch;
            var distance = Math.Max(AngleMath.DistanceRadians(this.StartYaw, this.TargetYaw), AngleMath.DistanceRadians(this.StartPich, this.TargetPitch));
            this.ETA = distance / radiansPerSecond;
        }

        public void Update(Seconds elapsed)
        {
            var progress = Easings.Interpolate(this.accumulator / this.ETA, Easings.Functions.QuadraticEaseInOut);
            var yaw = AngleMath.LerpRadians(this.StartYaw, this.TargetYaw, progress);
            var pitch = AngleMath.LerpRadians(this.StartPich, this.TargetPitch, progress);

            this.Pose.Rotate(yaw, pitch, this.Pose.Roll);

            this.accumulator += elapsed;

            this.Completed = this.accumulator >= this.ETA;
        }

        public float StartYaw { get; }
        public float TargetYaw { get; }
        public float StartPich { get; }
        public float TargetPitch { get; }
        public Seconds ETA { get; set; }

        public bool Completed { get; private set; }


        // TODO: simplify this class and move it to a different package

    }
}
