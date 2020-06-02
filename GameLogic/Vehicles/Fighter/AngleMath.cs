using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    // TODO: move to a different package
    public static class AngleMath
    {
        // TODO: simplify
        public static float LerpRadians(float a, float b, float lerpFactor) // Lerps from angle a to b (both between -PI and PI), taking the shortest path
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


        public static float DistanceRadians(float a, float b)
        {
            a = MathHelper.WrapAngle(a) + MathHelper.Pi;
            b = MathHelper.WrapAngle(b) + MathHelper.Pi;
            return Math.Abs(b - a);
        }

        // TODO: do not allow straight up!
        public static float YawFromVector(Vector3 targetDirection)
        {
            var v2 = Vector2.Normalize(new Vector2(targetDirection.Z, targetDirection.X));
            return MathHelper.WrapAngle((float)Math.Atan2(v2.Y, v2.X) + MathHelper.Pi);
        }

        // TODO: do not allow straight up!
        public static float PitchFromVector(Vector3 targetDirection)
        {
            // TODO: can this be simplified?
            var groundPlanePosition = new Vector2(targetDirection.X, targetDirection.Z);
            var groundDistanceFromOrigin = groundPlanePosition.Length();

            var v2 = Vector2.Normalize(new Vector2(-targetDirection.Y, groundDistanceFromOrigin));
            return MathHelper.WrapAngle((float)Math.Atan2(v2.Y, v2.X) - MathHelper.PiOver2);
        }
    }
}
