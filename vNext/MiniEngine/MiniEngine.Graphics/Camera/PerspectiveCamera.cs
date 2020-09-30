using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Camera
{
    public sealed class PerspectiveCamera : ICamera
    {
        public PerspectiveCamera(float aspectRatio)
        {
            this.AspectRatio = aspectRatio;
            this.Move(Vector3.Zero, Vector3.Forward);
        }

        public float NearPlane => 0.1f;

        public float FarPlane => 250.0f;

        public float FieldOfView => MathHelper.PiOver2;

        public float AspectRatio { get; private set; }

        public Vector3 Position { get; private set; }

        public Vector3 LookAt { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;

            var view = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
            var projection = Matrix.CreatePerspectiveFieldOfView(this.FieldOfView, this.AspectRatio, this.NearPlane, this.FarPlane);

            this.ViewProjection = view * projection;
        }

        public Matrix ViewProjection { get; private set; }
    }
}
