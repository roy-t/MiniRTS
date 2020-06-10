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
        private const float MinRotateDistance = 0.1f;
        private const float MaxLinearAcceleration = 1.0f;
        private const float MaxAngularAcceleration = 1.0f;

        private readonly Queue<IManeuver> Maneuvers;

        public FlightController(Pose pose)
        {
            this.Pose = pose;
            this.Maneuvers = new Queue<IManeuver>();
        }

        public Pose Pose { get; set; }

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
                    if (this.Maneuvers.Count > 0)
                    {
                        this.Maneuvers.Peek().Initiate();
                    }
                }
            }
            else
            {
                if (Vector3.Distance(this.Pose.Position, this.MoveTo) > MinMoveDistance)
                {
                    var targetDirection = Vector3.Normalize(this.MoveTo - this.Pose.Position);
                    var dot = Vector3.Dot(this.Pose.GetForward(), targetDirection);

                    if (Math.Abs(dot - 1.0f) > MinRotateDistance)
                    {
                        ManeuverPlanner.PlanPointAt(this.Maneuvers, this.Pose, this.MoveTo, MaxAngularAcceleration);
                    }
                    else
                    {
                        ManeuverPlanner.PlanMoveTo(this.Maneuvers, this.Pose, this.MoveTo, MaxLinearAcceleration, MaxAngularAcceleration);
                    }
                }
            }
        }
    }
}
