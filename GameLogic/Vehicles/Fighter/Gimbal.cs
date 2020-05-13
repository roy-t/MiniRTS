using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public sealed class Gimbal
    {
        private readonly Pose pose;

        public Gimbal(Pose pose)
        {
            this.pose = pose;
        }


        public Vector3 PointAt { get; set; }


        public void Update(Seconds elapsed)
        {
            var forward = Vector3.Forward;
            var desiredForward = Vector3.Normalize(this.PointAt - this.pose.Position);

            var dot = Vector3.Dot(forward, desiredForward);

            Vector3 axis;
            float angle;

            if (Math.Abs(dot - 1.0f) < 0.01f)
            {
                // vector a and b point in the same direction   
                return;
            }
            else if (Math.Abs(dot + 1.0f) < 0.01f)
            {
                // vector a and b point in the opposite direction, 
                // so it is a 180 degrees turn around the up-axis
                axis = Vector3.Up;
                angle = MathHelper.Pi;
            }
            else
            {
                axis = Vector3.Normalize(Vector3.Cross(forward, desiredForward));
                angle = (float)Math.Acos(dot);
            }

            (var yaw, var pitch, var roll) = ToEulerAngles(axis, angle);

            var speed = 1.0f; // elapsed * 1.0f;
            this.pose.Rotate(yaw * speed, pitch * speed, roll * speed);
        }

        private static (float yaw, float pitch, float roll) ToEulerAngles(Vector3 normalizedAxis, float angle)
        {
            var yaw = 0.0;
            var pitch = 0.0;
            var roll = 0.0;

            var x = normalizedAxis.X;
            var y = normalizedAxis.Y;
            var z = normalizedAxis.Z;

            var s = Math.Sin(angle);
            var c = Math.Cos(angle);
            var t = 1 - c;

            // North pole singularity
            if ((x * y * t + z * s) > 0.998)
            {
                yaw = (float)(2 * Math.Atan2(x * Math.Sin(angle / 2), Math.Cos(angle / 2)));
                pitch = MathHelper.PiOver2;
                roll = 0;
            }

            // South pole singularity
            if ((x * y * t + z * s) < -0.998)
            {
                yaw = (float)(-2 * Math.Atan2(x * Math.Sin(angle / 2), Math.Cos(angle / 2)));
                pitch = -MathHelper.PiOver2;
                roll = 0;
            }

            yaw = Math.Atan2(y * s - x * z * t, 1 - (y * y + z * z) * t);
            pitch = Math.Asin(x * y * t + z * s);
            roll = Math.Atan2(x * s - y * z * t, 1 - (x * x + z * z) * t);


            //return ((float)yaw, (float)pitch, (float)roll);
            return ((float)yaw, (float)roll, (float)pitch);
        }

    }
}
