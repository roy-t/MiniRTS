using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.GameLogic
{
    public sealed class Path
    {
        public Path(IReadOnlyList<Vector3> waypoints)
        {
            this.WayPoints = waypoints;
            this.Length = this.ComputePathLength();
        }

        public float Length { get; }

        public IReadOnlyList<Vector3> WayPoints { get; }

        public Vector3 GetPositionAfter(float distanceCovered)
        {
            var toCover = distanceCovered;
            for (var i = 1; i < this.WayPoints.Count; i++)
            {
                var from = this.GetPosition(i - 1);
                var to = this.GetPosition(i);

                var distance = Vector3.Distance(from, to);
                if (toCover > distance)
                {
                    toCover -= distance; ;
                }
                else
                {
                    return Vector3.Lerp(from, to, toCover / distance);
                }
            }

            return this.GetPosition(this.WayPoints.Count - 1);
        }

        private Vector3 GetPosition(int index) => this.WayPoints[index];

        private float ComputePathLength()
        {
            var length = 0.0f;
            for (var i = 1; i < this.WayPoints.Count; i++)
            {
                var from = this.GetPosition(i - 1);
                var to = this.GetPosition(i);

                var distance = Vector3.Distance(from, to);
                length += distance;
            }

            return length;
        }
    }
}
