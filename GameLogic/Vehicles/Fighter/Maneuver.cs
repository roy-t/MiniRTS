using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public class Maneuver
    {
        private readonly Pose Pose;
        private Seconds accumulator;

        public Maneuver(Pose pose, float targetYaw, float targetPitch, float radiansPerSecond)
        {
            this.Pose = pose;
            this.StartYaw = pose.Yaw;
            this.TargetYaw = targetYaw;
            this.StartPich = pose.Pitch;
            this.TargetPitch = targetPitch;
            var distance = Math.Max(DistanceRadians(this.StartYaw, this.TargetYaw), DistanceRadians(this.StartPich, this.TargetPitch));
            this.ETA = distance / radiansPerSecond;
        }

        public void Update(Seconds elapsed)
        {
            var progress = Easings.Interpolate(this.accumulator / this.ETA, Easings.Functions.QuadraticEaseInOut);
            var yaw = LerpRadians(this.StartYaw, this.TargetYaw, progress);
            var pitch = LerpRadians(this.StartPich, this.TargetPitch, progress);

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
        private static float LerpRadians(float a, float b, float lerpFactor) // Lerps from angle a to b (both between -PI and PI), taking the shortest path
        {

            if (a < MathHelper.Pi)
            {
                a += MathHelper.TwoPi;
            }

            if (b < MathHelper.Pi)
            {
                b += MathHelper.TwoPi;
            }


            float result;
            var diff = b - a;
            if (diff < -MathHelper.Pi)
            {
                // lerp upwards past MathHelper.TwoPi
                b += MathHelper.TwoPi;
                result = MathHelper.Lerp(a, b, lerpFactor);
            }
            else if (diff > MathHelper.Pi)
            {
                // lerp downwards past 0
                b -= MathHelper.TwoPi;
                result = MathHelper.Lerp(a, b, lerpFactor);
            }
            else
            {
                // straight lerp
                result = MathHelper.Lerp(a, b, lerpFactor);
            }

            return MathHelper.WrapAngle(result);
        }

        private static float DistanceRadians(float a, float b)
        {
            a = MathHelper.WrapAngle(a) + MathHelper.Pi;
            b = MathHelper.WrapAngle(b) + MathHelper.Pi;
            return Math.Abs(b - a);
        }
    }
}
