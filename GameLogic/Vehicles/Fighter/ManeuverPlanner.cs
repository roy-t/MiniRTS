using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public static class ManeuverPlanner
    {

        public static void PlanPointAt(Queue<IManeuver> maneuvers, Pose pose, Vector3 target, float maxAngularAcceleration)
        {
            var targetDirection = Vector3.Normalize(target - pose.Position);

            var targetYaw = AngleMath.YawFromVector(targetDirection);
            var yawDistance = AngleMath.DistanceRadians(pose.Yaw, targetYaw);
            var yawRotationDuration = ComputeRotationDuration(yawDistance, maxAngularAcceleration);

            var targetPitch = AngleMath.PitchFromVector(targetDirection);
            var pitchDistance = AngleMath.DistanceRadians(pose.Pitch, targetPitch);
            var pitchRotationDuration = ComputeRotationDuration(pitchDistance, maxAngularAcceleration);

            var rotation = new RotationManeuver(pose, Vector3.Zero, targetYaw, yawRotationDuration, targetPitch, pitchRotationDuration, maxAngularAcceleration);
            var adjust = new LerpManeuver(pose, pose.Position, targetYaw, targetPitch, new Seconds(0.2f));

            maneuvers.Enqueue(rotation);
            maneuvers.Enqueue(adjust);
        }

        public static void PlanMoveTo(Queue<IManeuver> maneuvers, Pose pose, Vector3 target, float maxLinearAcceleration, float maxAngularAcceleration)
        {
            // 1. Accelerate by performing a prograde burn
            // 2. Rotate so that the rocket points in the opposite direction of the velocity vector
            // 3. Decelerate by performing a retrograde burn
            // 4. Make tiny adjustments to correct floating point math errors

            var forward = pose.GetForward();
            var targetYaw = AngleMath.YawFromVector(-forward);
            var yawRotationDuration = ComputeRotationDuration(Math.PI, maxAngularAcceleration);

            var targetPitch = AngleMath.PitchFromVector(-forward);
            var pitchDistance = AngleMath.DistanceRadians(pose.Pitch, targetPitch);
            var pitchRotationDuration = ComputeRotationDuration(pitchDistance, maxAngularAcceleration);

            var distance = Vector3.Distance(pose.Position, target);
            var burnDuration = ComputeBurnDuration(distance, maxLinearAcceleration, yawRotationDuration);
            var velocityAfterBurn = maxLinearAcceleration * burnDuration * forward;


            var direction = Vector3.Normalize(target - pose.Position);
            var progradeBurn = new BurnManeuver(pose, direction, Vector3.Zero, maxLinearAcceleration, burnDuration);
            var rotation = new RotationManeuver(pose, velocityAfterBurn, targetYaw, yawRotationDuration, targetPitch, pitchRotationDuration, maxAngularAcceleration);
            var retrogradeBurn = new BurnManeuver(pose, -direction, velocityAfterBurn, maxLinearAcceleration, burnDuration);
            var adjust = new LerpManeuver(pose, target, targetYaw, targetPitch, new Seconds(1.0f));

            maneuvers.Enqueue(progradeBurn);
            maneuvers.Enqueue(rotation);
            maneuvers.Enqueue(retrogradeBurn);
            maneuvers.Enqueue(adjust);
        }

        private static float ComputeRotationDuration(double distanceRadians, double accelerationRadians)
        {
            var a = accelerationRadians;
            var b = 0;
            var c = -Math.Abs(distanceRadians);

            var d = (b * b) - (4 * a * c);

            return (float)((-b + Math.Sqrt(d)) / (2 * a)) * 2;
        }

        private static float ComputeBurnDuration(double distance, double acceleration, double rotationTime)
        {
            var a = acceleration;
            var b = acceleration * rotationTime;
            var c = -distance;

            var d = (b * b) - (4 * a * c);

            return (float)((-b + Math.Sqrt(d)) / (2 * a));
        }
    }
}
