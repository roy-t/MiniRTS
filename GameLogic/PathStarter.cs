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

            wayPoints.Add(startPosition);
            if (absStartAngle >= MathHelper.PiOver4 && absStartAngle < MathHelper.PiOver2)
            {
                // Drive backwards, have the front wheels follow a circle rotating in the opposite direction
                // of the way we are supposed to go. This makes the back wheels become angled in the direction
                // that we want to go.
                Vector3 offset;
                Vector3 circleBase;

                var carCenter = carDynamics.GetCarSupportedCenter();
                offset = carDynamics.GetCarLeft() * axleDistance * Math.Sign(startAngle);
                circleBase = carCenter + offset;

                for (var i = 0; i < 10; i++)
                {
                    // To get the rear wheels completely in the right direction we should follow 
                    // the entire difference in angle, but this will rotate the front wheels to 
                    // a relativel 90 degree angle. So we rotate only partially as we can make up
                    // the rest of the rotation by going forward
                    var rot = Matrix.CreateRotationY((startAngle / 17) * -i);
                    var o = Vector3.Transform(offset, rot);

                    wayPoints.Add(circleBase - o);
                }

                var last = wayPoints[wayPoints.Count - 1];
                wayPoints.RemoveAt(wayPoints.Count - 1);
                var backwards = Vector3.Normalize(last - path.WayPoints[1]);
                wayPoints.Add(last + (backwards * axleDistance * 0.1f));
                wayPoints.Add(last + (backwards * axleDistance * 0.2f));
            }

            for (var i = 1; i < path.WayPoints.Count; i++)
            {
                wayPoints.Add(path.WayPoints[i]);
            }

            return new Path(wayPoints);



            // TODO think of two circles, the first circle to get the rear wheels in the middle of a circle
            // the second circle to rotate to the desired position. Sort of what we cheat with above by adding a backwards part
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
