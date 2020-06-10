using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic.Vehicles.Fighter
{
    public static class ManeuverPlanner
    {
        private const float MinMoveDistance = 0.001f;
        private const float MinRotateDistance = MathHelper.TwoPi / 1000.0f;

        public static void PlanPointAt(Queue<IManeuver> maneuvers, Pose pose, Vector3 target, float maxAngularAcceleration)
        {
            if (Vector3.DistanceSquared(target, pose.Position) > 0)
            {
                var targetDirection = Vector3.Normalize(target - pose.Position);

                var targetYaw = AngleMath.YawFromVector(targetDirection);
                var yawDistance = AngleMath.DistanceRadians(pose.Yaw, targetYaw);

                var targetPitch = AngleMath.PitchFromVector(targetDirection);
                var pitchDistance = AngleMath.DistanceRadians(pose.Pitch, targetPitch);

                if (Math.Abs(yawDistance) > MinRotateDistance || Math.Abs(pitchDistance) > MinRotateDistance)
                {
                    var yawRotationDuration = ComputeRotationDuration(yawDistance, maxAngularAcceleration);
                    var pitchRotationDuration = ComputeRotationDuration(pitchDistance, maxAngularAcceleration);
                    var rotation = new RotationManeuver(Vector3.Zero, Math.Sign(yawDistance), yawRotationDuration, Math.Sign(pitchDistance), pitchRotationDuration, maxAngularAcceleration);
                    maneuvers.Enqueue(rotation);
                }

                var adjust = new LerpManeuver(pose.Position, targetYaw, targetPitch, new Seconds(0.2f));
                maneuvers.Enqueue(adjust);
            }
        }

        public static void PlanMoveTo(Queue<IManeuver> maneuvers, Pose pose, Vector3 target, float maxLinearAcceleration, float maxAngularAcceleration)
        {
            // 1. Point in the right direction
            // 2. Accelerate by performing a prograde burn
            // 3. Rotate so that the rocket points in the opposite direction of the velocity vector
            // 4. Decelerate by performing a retrograde burn
            // 5. Make tiny adjustments to correct floating point math errors
            PlanPointAt(maneuvers, pose, target, maxAngularAcceleration);

            var distance = Vector3.Distance(pose.Position, target);
            if (distance > MinMoveDistance)
            {
                var direction = Vector3.Normalize(target - pose.Position);

                var targetYaw = AngleMath.YawFromVector(-direction);
                var yawDistance = Math.PI;
                var yawRotationDuration = ComputeRotationDuration(yawDistance, maxAngularAcceleration);

                var currentPitch = AngleMath.PitchFromVector(direction);
                var targetPitch = AngleMath.PitchFromVector(-direction);
                var pitchDistance = AngleMath.DistanceRadians(currentPitch, targetPitch);
                var pitchRotationDuration = ComputeRotationDuration(pitchDistance, maxAngularAcceleration);

                var burnDuration = ComputeBurnDuration(distance, maxLinearAcceleration, yawRotationDuration);
                var velocityAfterBurn = maxLinearAcceleration * burnDuration * direction;

                var progradeBurn = new BurnManeuver(direction, Vector3.Zero, maxLinearAcceleration, burnDuration);
                var rotation = new RotationManeuver(velocityAfterBurn, Math.Sign(yawDistance), yawRotationDuration, Math.Sign(pitchDistance), pitchRotationDuration, maxAngularAcceleration);
                var retrogradeBurn = new BurnManeuver(-direction, velocityAfterBurn, maxLinearAcceleration, burnDuration);
                var adjust = new LerpManeuver(target, targetYaw, targetPitch, new Seconds(1.0f));

                maneuvers.Enqueue(progradeBurn);
                maneuvers.Enqueue(rotation);
                maneuvers.Enqueue(retrogradeBurn);
                maneuvers.Enqueue(adjust);
            }
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
