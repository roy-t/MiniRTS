using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public class Gimbal
    {
        private readonly Pose Pose;
        private readonly Queue<Maneuver> Maneuvers;

        public Gimbal(Pose pose)
        {
            this.Pose = pose;
            this.Maneuvers = new Queue<Maneuver>();
        }


        public Vector3 PointAt { get; set; }

        public void Update(Seconds elapsed)
        {
            if (this.Maneuvers.Count > 0)
            {
                var currentManeuver = this.Maneuvers.Peek();

                currentManeuver.Update(elapsed);
                if (currentManeuver.Completed)
                {
                    this.Maneuvers.Dequeue();
                }
            }
            else
            {
                // TODO: do not allow straight up, move to a different package
                var targetDirection = Vector3.Normalize(this.PointAt - this.Pose.Position);
                var dot = Vector3.Dot(this.Pose.GetForward(), targetDirection);

                if (targetDirection.LengthSquared() > 0 && Math.Abs(dot - 1.0f) > 0.01f)
                {
                    var yaw = GetYaw(targetDirection);
                    var pitch = GetPitch(targetDirection);

                    // TODO: base ETA on distance
                    var maneuver = new Maneuver(this.Pose, yaw, pitch, 1.0f);
                    this.Maneuvers.Enqueue(maneuver);
                }
            }
        }

        private static float GetYaw(Vector3 targetDirection)
        {
            var v2 = Vector2.Normalize(new Vector2(targetDirection.Z, targetDirection.X));
            return MathHelper.WrapAngle((float)Math.Atan2(v2.Y, v2.X) + MathHelper.Pi);
        }

        private static float GetPitch(Vector3 targetDirection)
        {
            // TODO: can this be simplified?
            var groundPlanePosition = new Vector2(targetDirection.X, targetDirection.Z);
            var groundDistanceFromOrigin = groundPlanePosition.Length();

            var v2 = Vector2.Normalize(new Vector2(-targetDirection.Y, groundDistanceFromOrigin));
            return MathHelper.WrapAngle((float)Math.Atan2(v2.Y, v2.X) - MathHelper.PiOver2);
        }
    }
}
