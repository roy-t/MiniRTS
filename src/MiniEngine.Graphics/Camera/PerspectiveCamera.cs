﻿using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Physics;

namespace MiniEngine.Graphics.Camera
{
    public sealed class PerspectiveCamera
    {
        private readonly Transform Transform;

        public PerspectiveCamera(float aspectRatio)
            : this(aspectRatio, Vector3.Zero, Vector3.Forward) { }

        public PerspectiveCamera(float aspectRatio, Vector3 position, Vector3 forward)
        {
            this.AspectRatio = aspectRatio;
            this.Transform = new Transform();
            this.Transform.MoveTo(position);
            this.Transform.FaceTargetConstrained(position + forward, Vector3.Up);

            this.Update();
        }

        public float NearPlane { get; } = 0.1f;

        public float FarPlane { get; } = 250.0f;

        public float FieldOfView { get; } = MathHelper.PiOver2;

        public float AspectRatio { get; }

        public Matrix ViewProjection { get; private set; }

        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }

        public Vector3 Position => this.Transform.Position;

        public Vector3 Forward => this.Transform.Forward;

        public Vector3 Up => this.Transform.Up;

        public Vector3 Left => this.Transform.Left;

        public void Update()
        {
            var position = this.Transform.Position;
            var forward = this.Transform.Forward;
            var up = this.Transform.Up;

            this.View = Matrix.CreateLookAt(position, position + forward, up);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlane, this.FarPlane);
            this.ViewProjection = this.View * this.Projection;
        }

        public void MoveTo(Vector3 position)
        {
            this.Transform.MoveTo(position);
            this.Update();
        }

        public void SetRotation(Quaternion rotation)
        {
            this.Transform.SetRotation(rotation);
            this.Update();
        }

        public void ApplyRotation(Quaternion rotation)
        {
            this.Transform.ApplyRotation(rotation);
            this.Update();
        }
        public void FaceTarget(Vector3 target)
        {
            this.Transform.FaceTarget(target);
            this.Update();
        }

        public void FaceTargetConstrained(Vector3 target, Vector3 up)
        {
            this.Transform.FaceTargetConstrained(target, up);
            this.Update();
        }
    }
}
