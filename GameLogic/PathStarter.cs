using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;

namespace MiniEngine.GameLogic
{
    public static class PathStarter
    {
        public static Path CreateStart(Path path, AModel car)
        {
            // TODO: special case waypoint with just 1 point?
            // get the car in the center of its current tile
            if (path.WayPoints.Count < 2)
            {
                return path;
            }

            // TODO: don't create these here
            var carDynamics = new CarDynamics(new CarLayout(car));
            var axleDistance = carDynamics.AxleDistance;
            var wayPoints = new List<Vector3>(path.WayPoints.Count);

            var startPosition = carDynamics.GetCarProjectedFrontAxlePosition();
            var startForward = carDynamics.GetCarForward();
            var startLeft = carDynamics.GetCarLeft();

            var startAngle = GetAngleDifference(startPosition, startForward, startLeft, path.WayPoints[1]);
            var absStartAngle = Math.Abs(startAngle);

            var carCenter = carDynamics.GetCarSupportedCenter();

            wayPoints.Add(startPosition);
            if (absStartAngle >= MathHelper.PiOver4)
            {
                // Drive backwards, have the front wheels follow a circle rotating in the opposite direction
                // of the way we are supposed to go. This makes the back wheels become angled in the direction
                // that we want to go.   

                wayPoints.Add(carCenter);


                var sign = Math.Sign(startAngle);
                var length = 0.5f;

                (var startPoint, var circleBase) = ComputeStartPointOfCircle(axleDistance, wayPoints, sign);
                var pointsOnCircle = CreateCircle(circleBase, startPoint, startAngle * length, 10);
                wayPoints.AddRange(pointsOnCircle);

                sign = -sign;

                (startPoint, circleBase) = ComputeStartPointOfCircle(axleDistance, wayPoints, sign);
                var pointsOnSecondCircle = CreateCircle(circleBase, startPoint, -startAngle * length, 10);
                wayPoints.AddRange(pointsOnSecondCircle);
            }

            //for (var i = 1; i < path.WayPoints.Count; i++)
            //{
            //    wayPoints.Add(path.WayPoints[i]);
            //}

            return new Path(wayPoints);



            // TODO think of two circles, the first circle to get the rear wheels in the middle of a circle
            // the second circle to rotate to the desired position. Sort of what we cheat with above by adding a backwards part
        }

        private static (Vector3 startPoint, Vector3 pointOnPerimeter) ComputeStartPointOfCircle(float axleDistance, List<Vector3> wayPoints, float sign)
        {
            var a = wayPoints[wayPoints.Count - 2];
            var startPoint = wayPoints[wayPoints.Count - 1];
            var to = Vector3.Normalize(startPoint - a);
            var cross = Vector3.Cross(to, Vector3.Up);

            var circleBase = startPoint + (cross * axleDistance * sign);

            return (startPoint, circleBase);
        }

        private static List<Vector3> CreateCircle(Vector3 origin, Vector3 startPointOnPerimeter, float angle, int steps)
        {
            var wayPoints = new List<Vector3>(steps);

            var offset = startPointOnPerimeter - origin;

            var stepSize = angle / steps;
            for (var i = 0; i < steps; i++)
            {
                var rotation = Matrix.CreateRotationY(stepSize * -i);
                var pointOnPerimeter = origin + Vector3.Transform(offset, rotation);

                wayPoints.Add(pointOnPerimeter);
            }

            return wayPoints;
        }


        private static float GetAngleDifference(Vector3 carPosition, Vector3 carForward, Vector3 carLeft, Vector3 worldPosition)
        {
            var carAngle = (float)Math.Atan2(carForward.Z, carForward.X);

            var toWorld = Vector3.Normalize(worldPosition - carPosition);
            var worldAngle = (float)Math.Atan2(toWorld.Z, toWorld.X);

            var diff = MathHelper.WrapAngle(carAngle - worldAngle);

            return diff;
        }
    }
}
