using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic
{
    public sealed class DumbFollowLogic
    {
        private AModel target;
        private List<Vector2> path;
        private float length;

        private float distanceCovered;
        private MetersPerSecond speed;

        public DumbFollowLogic()
        {
            this.length = 0.0f;
            this.distanceCovered = 0.0f;
        }

        public void Start(AModel target, List<Vector2> path, MetersPerSecond speed)
        {
            this.target = target;
            this.path = path;
            this.speed = speed;
            this.distanceCovered = 0.0f;

            var length = 0.0f;
            for (var i = 1; i < this.path.Count; i++)
            {
                var from = this.GetPosition(i - 1);
                var to = this.GetPosition(i);

                var distance = Vector3.Distance(from, to);
                length += distance;
            }

            this.length = length;
        }


        public void Update(Seconds elapsed)
        {
            if (this.distanceCovered >= this.length)
                return;

            this.distanceCovered += elapsed * this.speed;
            var position = this.GetPositionAfter(this.distanceCovered);
            var lookAt = this.GetPositionAfter(this.distanceCovered + 0.2f);

            this.target.Move(position);


            var n = Vector3.Normalize(lookAt - position);
            if (n.LengthSquared() > 0)
            {
                var yaw = (float)Math.Atan2(n.X, n.Z) - MathHelper.PiOver2;
                this.target.Yaw = yaw;
            }
        }

        private Vector3 GetPositionAfter(float distanceCovered)
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
    }
}
