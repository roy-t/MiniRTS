using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Mutators.Functions
{
    public sealed class FollowPathFunction : IMutatorFunction<Matrix>
    {
        private readonly IReadOnlyList<Vector3> Waypoints;

        private float accumulator;

        public FollowPathFunction(IReadOnlyList<Vector3> waypoints, float duration)
        {
            this.Duration = duration;
            this.Waypoints = waypoints;

            var distance = 0.0f;
            for (var i = 1; i < this.Waypoints.Count; i++)
            {
                var from = this.Waypoints[i - 1];
                var to = this.Waypoints[i];
                distance += Vector3.Distance(from, to);
            }

            this.Distance = distance;
        }

        public float Duration { get; set; }

        public float Distance { get; }

        public Matrix Update(float elapsed, Matrix value)
        {
            this.accumulator = (this.accumulator + elapsed) % this.Duration;
            var progress = this.accumulator / this.Duration;
            var traveled = this.Distance * progress;

            var distance = 0.0f;
            for (var i = 1; i < this.Waypoints.Count; i++)
            {
                var from = this.Waypoints[i - 1];
                var to = this.Waypoints[i];
                var leg = Vector3.Distance(from, to); ;

                if (distance + leg > traveled)
                {
                    var offset = (traveled - distance) / leg;

                    value.Translation = Vector3.Lerp(from, to, offset);
                    break;
                }
                else
                {
                    distance += leg;
                }
            }

            return value;
        }
    }
}
