using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public class FlightController
    {
        private const float MinMoveDistance = 0.1f;
        private const float MaxLinearAcceleration = 1.0f;
        private const float MaxAngularAcceleration = 1.0f;

        private readonly Pose Pose;
        private readonly Queue<IManeuver> Maneuvers;

        public FlightController(Pose pose)
        {
            this.Pose = pose;
            this.Maneuvers = new Queue<IManeuver>();
        }

        public Vector3 PointAt { get; set; }
        public Vector3 MoveTo { get; set; }

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
                if (Vector3.Distance(this.Pose.Position, this.MoveTo) > MinMoveDistance)
                {
                    var targetDirection = Vector3.Normalize(this.PointAt - this.Pose.Position);
                    var dot = Vector3.Dot(this.Pose.GetForward(), targetDirection);

                    if (targetDirection.LengthSquared() > 0 && Math.Abs(dot - 1.0f) > 0.01f)
                    {
                        var yaw = AngleMath.YawFromVector(targetDirection);
                        var pitch = AngleMath.PitchFromVector(targetDirection);

                        var rotation = new RotationManeuver(this.Pose, yaw, pitch, MathHelper.TwoPi / 10);
                        this.Maneuvers.Enqueue(rotation);
                    }
                    else
                    {
                        var translation = new BurnRetroBurnManeuver(this.Pose, this.MoveTo, MaxLinearAcceleration, MaxAngularAcceleration);
                        this.Maneuvers.Enqueue(translation);
                    }
                }
            }
        }


    }
}
