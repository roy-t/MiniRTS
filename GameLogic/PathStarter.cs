using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;

namespace MiniEngine.GameLogic
{
    public static class PathStarter
    {
        public static Path CreateStart(Path path, AModel car)
        {
            if (path.WayPoints.Count < 2)
            {
                return path;
            }

            // TODO: don't create these here
            var carDynamics = new CarDynamics(new CarLayout(car));

            var startPosition = carDynamics.GetCarProjectedFrontAxlePosition();
            var startForward = carDynamics.GetCarForward();

            var wayPoints = new List<Vector3>(path.WayPoints.Count);

            var w0 = new Vector3(startPosition.X, 0, startPosition.Z);
            var w1 = w0 + (startForward * carDynamics.AxleDistance);

            wayPoints.Add(w0);
            wayPoints.Add(w1);
            wayPoints.Add(path.WayPoints[path.WayPoints.Count - 1]);

            //for (var i = 2; i < path.WayPoints.Count; i++)
            //{
            //    wayPoints.Add(path.WayPoints[i]);
            //}

            return new Path(wayPoints);
        }
    }
}
