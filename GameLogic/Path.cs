using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.GameLogic
{
    public sealed class Path
    {
        private readonly IReadOnlyList<Vector2> path;

        public Path(IReadOnlyList<Vector2> path)
        {
            this.path = path;
            this.Length = this.ComputePathLength();
        }

        public float Length { get; }

        public Vector3 GetPositionAfter(float distanceCovered)
        {
            var toCover = distanceCovered;
            for (var i = 1; i < this.path.Count; i++)
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

            return this.GetPosition(this.path.Count - 1);
        }

        private Vector3 GetPosition(int index)
        {
            var v2 = this.path[index];
            return new Vector3(v2.X, 0, v2.Y);
        }

        private float ComputePathLength()
        {
            var length = 0.0f;
            for (var i = 1; i < this.path.Count; i++)
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
