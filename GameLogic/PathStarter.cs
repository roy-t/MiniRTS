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

            var startAngle = GetAngleDifference(carDynamics, path.WayPoints[1]);
            var absStartAngle = Math.Abs(startAngle);


            wayPoints.Add(startPosition);
            var maxWheelAngle = (MathHelper.Pi / 6.0f);
            if (absStartAngle >= MathHelper.PiOver4)
            {
                // TODO: infinite loop if the point is very close

                var steps = 20;
                var rotationStepSize = (MathHelper.Pi / 8) * Math.Sign(-startAngle);
                var translationStepSize = axleDistance / steps;
                do
                {
                    var forward = carDynamics.GetCarForward();

                    var rotation = Matrix.CreateRotationY(rotationStepSize);
                    var newForward = Vector3.Transform(forward, rotation);

                    var newFrontAxlePosition = carDynamics.GetCarProjectedFrontAxlePosition() - (newForward * translationStepSize);
                    carDynamics.BringAxlesInLine(newFrontAxlePosition);

                    wayPoints.Add(carDynamics.GetCarProjectedFrontAxlePosition());
                } while (Math.Abs(GetAngleDifference(carDynamics, path.WayPoints[1])) > (maxWheelAngle * 2));

                var halfWayIndex = wayPoints.Count - 1;

                do
                {
                    var forward = carDynamics.GetCarForward();

                    var rotation = Matrix.CreateRotationY(-rotationStepSize);
                    var newForward = Vector3.Transform(forward, rotation);

                    var newFrontAxlePosition = carDynamics.GetCarProjectedFrontAxlePosition() + (newForward * translationStepSize);
                    carDynamics.BringAxlesInLine(newFrontAxlePosition);

                    wayPoints.Add(carDynamics.GetCarProjectedFrontAxlePosition());
                } while (Math.Abs(GetAngleDifference(carDynamics, path.WayPoints[1])) > maxWheelAngle);

                var basis = wayPoints[halfWayIndex];
                var before = Vector3.Normalize(wayPoints[halfWayIndex] - wayPoints[halfWayIndex - 1]);
                var after = Vector3.Normalize(wayPoints[halfWayIndex + 1] - wayPoints[halfWayIndex + 2]);

                var cross = Vector3.Lerp(before, after, 0.5f);
                wayPoints.Insert(halfWayIndex, basis + (cross * axleDistance * 0.1f));
                wayPoints.Insert(halfWayIndex + 1, basis + (cross * axleDistance * 0.2f));
                wayPoints.Insert(halfWayIndex + 2, basis + (cross * axleDistance * 0.1f));
            }

            for (var i = 1; i < path.WayPoints.Count; i++)
            {
                wayPoints.Add(path.WayPoints[i]);
            }

            return new Path(wayPoints);
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


        private static float GetAngleDifference(CarDynamics carDynamics, Vector3 worldPosition)
        {
            var carPosition = carDynamics.GetCarProjectedFrontAxlePosition();
            var carForward = carDynamics.GetCarForward();
            var carLeft = carDynamics.GetCarLeft();

            var carAngle = (float)Math.Atan2(carForward.Z, carForward.X);

            var toWorld = Vector3.Normalize(worldPosition - carPosition);
            var worldAngle = (float)Math.Atan2(toWorld.Z, toWorld.X);

            var diff = MathHelper.WrapAngle(carAngle - worldAngle);

            return diff;
        }
    }
}
