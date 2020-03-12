using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Units;

namespace MiniEngine.GameLogic
{
    public sealed class DumbFollowLogic
    {
        private CarAnimation carAnimation;

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

            this.carAnimation = this.target.Animation as CarAnimation;
        }


        public void Update(Seconds elapsed)
        {
            // TODO: wheels should point so that they cross the vector going from the center of the front axis to the lookAt.
            // TODO: where to place the pivot of the car? At the back axis I assume?

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


            var fl = GetWorldPosition("Bone_FL");
            var fr = GetWorldPosition("Bone_FR");

            var frontAxisCenter = Vector3.Lerp(fl, fr, 0.5f);

            var axisForward = Vector3.Normalize(lookAt - frontAxisCenter);

            var wheelYaw = (float)Math.Atan2(axisForward.X, axisForward.Z);
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

        public Vector3 GetWorldPosition(string boneName)
        {
            //var skinningData = this.target.Model.Tag as SkinningData;

            //var index = skinningData.BoneNames.IndexOf(boneName);
            //var matrix = skinningData.BindPose[index] * this.target.Pose.Matrix;

            //return Vector3.Transform(Vector3.Zero, matrix);

            //this.CarAnimation.Update(0);
            var matrix = this.carAnimation.GetTo(boneName) * this.target.Pose.Matrix;

            //var index = this.CarAnimation.GetBoneIndex(boneName);
            //var matrix = this.CarAnimation.GetBoneTransforms()[index] * this.target.Pose.Matrix;

            return Vector3.Transform(Vector3.Zero, matrix);
        }

        private Vector3 GetPosition(int index)
        {
            var v2 = this.path[index];
            return new Vector3(v2.X, 0, v2.Y);
        }
    }
}
