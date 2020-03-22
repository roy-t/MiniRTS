using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.GameLogic
{
    public sealed class PathInterpolator
    {
        private const float l = 0.25f;

        public static IReadOnlyList<Vector2> Interpolate(IReadOnlyList<Vector2> path)
        {
            if (path.Count > 0)
            {
                var points = new List<Vector2>(path);
                var start = points[0];
                for (var r = 0; r < 5; r++)
                {
                    points = InterpolateInternal(points);
                }

                points.Insert(0, start);
                return points;
            }

            return path;
        }

        private static List<Vector2> InterpolateInternal(IReadOnlyList<Vector2> path)
        {
            var points = new List<Vector2>();

            for (var i = 2; i < path.Count; i += 2)
            {
                var start = path[i - 2];
                var control = path[i - 1];
                var end = path[i];

                points.AddRange(Interpolate(start, control, end));
            }

            return points;
        }

        private static Vector2[] Interpolate(Vector2 start, Vector2 control, Vector2 end)
        {
            var a = Vector2.Lerp(start, control, l);
            var b = Vector2.Lerp(start, control, l * 2);
            var c = Vector2.Lerp(start, control, l * 3);

            var a2 = Vector2.Lerp(control, end, l);
            var b2 = Vector2.Lerp(control, end, l * 2);
            var c2 = Vector2.Lerp(control, end, l * 3);


            var p0 = Vector2.Lerp(a, a2, l);
            var p1 = Vector2.Lerp(b, b2, l * 2);
            var p2 = Vector2.Lerp(c, c2, l * 3);


            return new Vector2[] { p0, p1, p2 };
        }
    }
}
