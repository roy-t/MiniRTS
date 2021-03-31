using System;
using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Physics
{
    public sealed class Transform
    {
        private Matrix matrix;
        private Quaternion rotation;
        private Vector3 origin;
        private Vector3 position;
        private Vector3 forward;
        private Vector3 scale;

        public Transform()
            : this(Vector3.Zero) { }

        public Transform(Vector3 position)
            : this(position, Vector3.One, Quaternion.Identity) { }

        public Transform(Vector3 position, float scale)
            : this(position, Vector3.One * scale, Quaternion.Identity) { }

        public Transform(Vector3 position, Vector3 scale)
            : this(position, scale, Quaternion.Identity) { }

        public Transform(Vector3 position, Vector3 scale, Quaternion rotation, Vector3 origin = default)
        {
            this.position = position;
            this.scale = scale;
            this.rotation = rotation;
            this.origin = origin;

            this.Recompute();
        }

        public Matrix Matrix => this.matrix;
        public Quaternion Rotation => this.rotation;
        public Vector3 Origin => this.origin;
        public Vector3 Position => this.position;
        public Vector3 Forward => this.forward;
        public Vector3 Scale => this.scale;

        public void MoveTo(Vector3 position)
        {
            this.position = position;
            this.Recompute();
        }

        public void SetScale(float scale)
            => this.SetScale(Vector3.One * scale);

        public void SetScale(Vector3 scale)
        {
            this.scale = scale;
            this.Recompute();
        }

        public void SetOrigin(Vector3 origin)
        {
            this.origin = origin;
            this.Recompute();
        }

        public void ApplyRotation(Quaternion rotation)
        {
            this.rotation = rotation * this.rotation;
            this.Recompute();
        }

        public void FaceTarget(Vector3 target)
        {
            var newForward = Vector3.Normalize(target - this.position);
            var rotation = GetRotation(this.forward, newForward, Vector3.Up);
            this.ApplyRotation(rotation);
        }

        private void Recompute()
        {
            this.forward = Vector3.Transform(Vector3.Forward, this.rotation);
            this.matrix = Combine(this.position, this.scale, this.origin, this.rotation);
        }

        private static Matrix Combine(Vector3 position, Vector3 scale, Vector3 origin, Quaternion rotation)
        {
            var moveToCenter = Matrix.CreateTranslation(-origin);
            var size = Matrix.CreateScale(scale);
            var translation = Matrix.CreateTranslation(position);

            var rotationMatrix = Matrix.CreateFromQuaternion(rotation);
            return size * moveToCenter * rotationMatrix * translation;
        }

        private static Quaternion GetRotation(Vector3 currentForward, Vector3 desiredForward, Vector3 up)
        {
            var dot = Vector3.Dot(currentForward, desiredForward);

            if (Math.Abs(dot + 1.0f) < 0.000001f)
            {
                // vector a and b point exactly in the opposite direction, 
                // so it is a 180 degrees turn around the up-axis
                return new Quaternion(up, MathHelper.Pi);
            }
            if (Math.Abs(dot - 1.0f) < 0.000001f)
            {
                // vector a and b point exactly in the same direction
                // so we return the identity quaternion
                return Quaternion.Identity;
            }

            var rotAngle = (float)Math.Acos(dot);
            var rotAxis = Vector3.Cross(currentForward, desiredForward);
            rotAxis = Vector3.Normalize(rotAxis);
            return Quaternion.CreateFromAxisAngle(rotAxis, rotAngle);
        }
    }
}
