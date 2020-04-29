using System;
using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;

namespace MiniEngine.GameLogic.Vehicles
{
    public sealed class Tank
    {
        private const int LeftTrackIndex = 1;
        private const int RightTrackIndex = 0;
        private const float TrackLengthModifier = 1.1f;

        private readonly AModel Model;
        private readonly UVAnimation Animation;
        private readonly Bounds Bounds;

        public Tank(AModel model, Bounds bounds, Pose pose, UVAnimation animation)
        {
            this.Model = model;
            this.Pose = pose;
            this.Animation = animation;
            this.Bounds = bounds;
        }

        public Pose Pose { get; }

        public void MoveAndTurn(Vector3 position, float yaw)
        {
            var length = (this.Bounds.BoundingBox.Max.Z - this.Bounds.BoundingBox.Min.Z);
            var unscaledWidth = (this.Bounds.BoundingBox.Max.Z - this.Bounds.BoundingBox.Min.Z) / this.Pose.Scale.X;

            var oldLeftOf = Vector3.Transform(Vector3.Left * unscaledWidth * 0.5f, this.Pose.Matrix);
            var oldRightOf = Vector3.Transform(Vector3.Right * unscaledWidth * 0.5f, this.Pose.Matrix);

            this.Pose.Move(position);
            this.Pose.Rotate(yaw, this.Pose.Pitch, this.Pose.Roll);

            var newLeftOf = Vector3.Transform(Vector3.Left * unscaledWidth * 0.5f, this.Pose.Matrix);
            var newRightOf = Vector3.Transform(Vector3.Right * unscaledWidth * 0.5f, this.Pose.Matrix);


            var leftDistance = Vector3.Distance(oldLeftOf, newLeftOf) * this.ComputeDirection(oldLeftOf, newLeftOf);
            var rightDistance = Vector3.Distance(oldRightOf, newRightOf) * this.ComputeDirection(oldRightOf, newRightOf);

            // Because the track is curved so its slightly longer than the length of the tank
            var leftFraction = leftDistance / (length * TrackLengthModifier);
            var rightFraction = rightDistance / (length * TrackLengthModifier);

            Console.WriteLine($"L: {leftDistance:F3} R: {rightDistance:F3}");
            this.RotateTrack(leftFraction, rightFraction);
        }

        private float ComputeDirection(Vector3 from, Vector3 to)
        {
            var direction = Vector3.Normalize(to - from);
            var forward = this.Pose.GetForward();

            var directionAngle = (float)Math.Atan2(direction.X, direction.Z) + MathHelper.Pi;
            var forwardAngle = (float)Math.Atan2(forward.X, forward.Z) + MathHelper.Pi;

            var diff = Math.Abs(MathHelper.WrapAngle(forwardAngle - directionAngle));

            if (diff <= MathHelper.PiOver2)
            {
                return 1.0f;
            }

            return -1.0f;
        }

        public void RotateTrack(float leftChange, float rightChange)
        {
            this.Animation.MeshUVOffsets[LeftTrackIndex].UVOffset += Vector2.UnitY * leftChange;
            this.Animation.MeshUVOffsets[RightTrackIndex].UVOffset += Vector2.UnitY * rightChange;
        }
    }
}

