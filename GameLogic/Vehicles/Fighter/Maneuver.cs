using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Utilities;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public class Maneuver
    {

        private Seconds accumulator;
        private readonly Pose pose;

        public Maneuver(Pose pose, float targetYaw, float targetPitch, Seconds eta)
        {
            this.pose = pose;
            this.StartYaw = pose.Yaw;
            this.TargetYaw = targetYaw;
            this.StartPich = pose.Pitch;
            this.TargetPitch = targetPitch;
            this.ETA = eta;
        }

        public void Update(Seconds elapsed)
        {
            var progress = Easings.Interpolate(this.accumulator / this.ETA, Easings.Functions.QuarticEaseOut);
            var yaw = LerpRadians(this.StartYaw, this.TargetYaw, progress);
            var pitch = LerpRadians(this.StartPich, this.TargetPitch, progress);

            this.pose.Rotate(yaw, pitch, this.pose.Roll);

            this.accumulator += elapsed;

            this.Completed = this.accumulator >= this.ETA;
        }

        public float StartYaw { get; }
        public float TargetYaw { get; }
        public float StartPich { get; }
        public float TargetPitch { get; }
        public Seconds ETA { get; }

        public bool Completed { get; private set; }


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
                if (result >= MathHelper.TwoPi)
                {
                    result -= MathHelper.TwoPi;
                }
            }
            else if (diff > MathHelper.Pi)
            {
                // lerp downwards past 0
                b -= MathHelper.TwoPi;
                result = MathHelper.Lerp(a, b, lerpFactor);
                if (result < 0.0f)
                {
                    result += MathHelper.TwoPi;
                }
            }
            else
            {
                // straight lerp
                result = MathHelper.Lerp(a, b, lerpFactor);
            }

            return MathHelper.WrapAngle(result);
        }
    }
}
