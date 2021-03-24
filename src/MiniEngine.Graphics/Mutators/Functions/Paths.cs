using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Mutators.Functions
{
    public static class Paths
    {
        public static FollowPathFunction Circle(Vector3 origin, float radius, float duration, int segments)
        {
            var waypoints = new List<Vector3>();

            for (var i = 0; i < segments; i++)
            {
                var x = Math.Cos((Math.PI * 2 / segments) * i);
                var z = Math.Sin((Math.PI * 2 / segments) * i);

                var position = new Vector3((float)x * radius, 0, (float)z * radius) + origin;
                waypoints.Add(position);
            }

            return new FollowPathFunction(waypoints, duration);
        }
    }
}
