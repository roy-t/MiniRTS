using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Primitives
{
    public struct Pose
    {
        public Pose(Vector3 position, Vector3 scale, float yaw, float pitch, float roll)
        {
            this.Translation = position;
            this.Scale = scale;
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;

            this.Matrix = Recompute(position, scale, yaw, pitch, roll);
        }

        public Pose(Vector3 position, float scale = 1.0f, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
            : this(position, Vector3.One * scale, yaw, pitch, roll) { }

        public Pose(Matrix matrix)
        {
            matrix.Decompose(out var scale, out var q, out var translation);

            // TODO: verify this is 100% correct
            this.Pitch = (float)Math.Atan2(2.0 * ((q.Y * q.Z) + (q.W * q.X)), (q.W * q.W) - (q.X * q.X) - (q.Y * q.Y) + (q.Z * q.Z));
            this.Yaw = (float)Math.Asin(-2.0 * ((q.X * q.Z) - (q.W * q.Y)));
            this.Roll = (float)Math.Atan2(2.0 * ((q.X * q.Y) + (q.W * q.Z)), (q.W * q.W) + (q.X * q.X) - (q.Y * q.Y) - (q.Z * q.Z));

            this.Translation = translation;
            this.Scale = scale;

            this.Matrix = matrix;
        }

        public Matrix Matrix { get; private set; }
        public float Yaw { get; private set; }
        public float Pitch { get; private set; }
        public float Roll { get; private set; }

        public Vector3 Translation { get; private set; }
        public Vector3 Scale { get; private set; }

        public Vector3 GetForward()
        {
            var rotation = Matrix.CreateRotationY(this.Yaw);
            return Vector3.Transform(Vector3.Forward, rotation);
        }

        public void Rotate(float yaw, float pitch, float roll)
        {
            this.Yaw = yaw;
            this.Pitch = pitch;
            this.Roll = roll;
            this.Matrix = Recompute(this.Translation, this.Scale, this.Yaw, this.Pitch, this.Roll);
        }

        public void Move(Vector3 position)
        {
            this.Translation = position;
            this.Matrix = Recompute(this.Translation, this.Scale, this.Yaw, this.Pitch, this.Roll);
        }

        public void SetScale(Vector3 scale)
        {
            this.Scale = scale;
            this.Matrix = Recompute(this.Translation, this.Scale, this.Yaw, this.Pitch, this.Roll);
        }

        public void SetScale(float scale) => this.SetScale(Vector3.One * scale);

        private static Matrix Recompute(Vector3 position, Vector3 scale, float yaw, float pitch, float roll)
        {
            var rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            var size = Matrix.CreateScale(scale);
            var translation = Matrix.CreateTranslation(position);

            return size * rotation * translation;
        }
    }
}
