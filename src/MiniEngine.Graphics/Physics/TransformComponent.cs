using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Physics
{
    public sealed class TransformComponent : AComponent
    {
        private readonly Queue<Vector3> TranslationQueue;
        private readonly Queue<Quaternion> RotationQueue;
        private readonly Queue<Vector3> ScaleQueue;
        private Matrix matrix;
        private Quaternion rotation;
        private Vector3 position;
        private Vector3 forward;
        private Vector3 scale;

        public TransformComponent(Entity entity)
            : this(entity, Vector3.Zero, Vector3.One, Quaternion.Identity) { }


        [Obsolete("Please upgrade")]
        public TransformComponent(Entity entity, Matrix matrix)
            : this(entity)
        {
            matrix.Decompose(out var scale, out var rotation, out var translation);

            this.TranslationQueue = new Queue<Vector3>();
            this.TranslationQueue.Enqueue(translation);
            this.position = Vector3.Zero;

            this.RotationQueue = new Queue<Quaternion>();
            this.RotationQueue.Enqueue(rotation);
            this.rotation = Quaternion.Identity;

            this.ScaleQueue = new Queue<Vector3>();
            this.ScaleQueue.Enqueue(scale);
            this.scale = Vector3.One;
        }

        public TransformComponent(Entity entity, Vector3 translation, Vector3 scale, Quaternion rotation)
            : base(entity)
        {
            this.TranslationQueue = new Queue<Vector3>();
            this.TranslationQueue.Enqueue(translation);
            this.position = Vector3.Zero;

            this.RotationQueue = new Queue<Quaternion>();
            this.RotationQueue.Enqueue(rotation);
            this.rotation = Quaternion.Identity;

            this.ScaleQueue = new Queue<Vector3>();
            this.ScaleQueue.Enqueue(scale);
            this.scale = Vector3.One;
        }

        public Matrix Matrix => this.matrix;
        public Quaternion Rotation => this.rotation;

        public Vector3 Position => this.position;
        public Vector3 Forward => this.forward;
        public Vector3 Scale => this.scale;

        public void Push(Vector3 relativeMovement)
        {
            this.TranslationQueue.Enqueue(relativeMovement);
            this.ChangeState.Change();
        }

        public void Turn(Quaternion relativeRotation)
        {
            this.RotationQueue.Enqueue(relativeRotation);
            this.ChangeState.Change();
        }

        public void Stretch(Vector3 additionalScale)
        {
            this.ScaleQueue.Enqueue(additionalScale);
            this.ChangeState.Change();
        }

        public void MoveTo(Vector3 nextPosition)
            => this.Push(nextPosition - this.position);


        public void FaceTarget(Vector3 target)
        {
            var newForward = Vector3.Normalize(target - this.position);
            var rotation = GetRotation(this.forward, newForward, Vector3.Up);
            this.Turn(rotation);
        }

        public void ProcessQueue()
        {
            while (this.TranslationQueue.TryDequeue(out var additionalTranslation))
            {
                this.position += additionalTranslation;
            }

            while (this.ScaleQueue.TryDequeue(out var additionalScale))
            {
                this.scale *= additionalScale;
            }

            while (this.RotationQueue.TryDequeue(out var additionalRotation))
            {
                this.rotation *= additionalRotation;
            }

            this.forward = Vector3.Transform(Vector3.Forward, this.rotation);
            this.matrix = Combine(this.position, this.scale, Vector3.Zero, this.rotation);
        }

        public static Matrix Combine(Vector3 position, Vector3 scale, Vector3 origin, Quaternion rotation)
        {
            var moveToCenter = Matrix.CreateTranslation(-origin);
            var size = Matrix.CreateScale(scale);
            var translation = Matrix.CreateTranslation(position);

            var rotationMatrix = Matrix.CreateFromQuaternion(rotation);
            return size * moveToCenter * rotationMatrix * translation;
        }

        public static Matrix Combine(Vector3 position, Vector3 scale, Vector3 origin, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
        {
            var moveToCenter = Matrix.CreateTranslation(-origin);
            var size = Matrix.CreateScale(scale);
            var translation = Matrix.CreateTranslation(position);

            var rotationMatrix = Matrix.CreateFromYawPitchRoll(yaw, pitch, roll);
            return size * moveToCenter * rotationMatrix * translation;
        }

        public static Matrix Combine(Vector3 position, Vector3 scale, float yaw = 0.0f, float pitch = 0.0f, float roll = 0.0f)
            => Combine(position, scale, Vector3.Zero, yaw, pitch, roll);


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
