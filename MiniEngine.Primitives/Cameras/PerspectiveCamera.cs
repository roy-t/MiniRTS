using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives.Cameras
{
    public sealed class PerspectiveCamera : IMovableViewPoint
    {
        private const float Epsilon = 0.001f;

        public PerspectiveCamera(Viewport viewport)
        {
            this.NearPlane = 0.1f;
            this.FarPlane = 250.0f;
            this.AspectRatio = viewport.AspectRatio;            

            this.Move(Vector3.Backward * 10, Vector3.Zero);
            this.SetFieldOfView(MathHelper.PiOver2);
        }

        public float NearPlane { get; }
        public float FarPlane { get; }
        public float AspectRatio { get; }
        public float FieldOfView { get; private set;}
        public Vector3 LookAt { get; private set; }
        public Matrix ViewProjection { get; private set; }

        public Matrix InverseViewProjection { get; private set; }

        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public BoundingFrustum Frustum { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;
            this.Forward = Vector3.Normalize(lookAt - position);

            this.View = Matrix.CreateLookAt(this.Position, this.LookAt, Vector3.Up);           

            this.ComputeMatrices();            
        }

        public void SetFieldOfView(float fieldOfView)
        {
            this.FieldOfView = MathHelper.Clamp(fieldOfView, Epsilon, MathHelper.Pi - Epsilon);

            this.Projection = Matrix.CreatePerspectiveFieldOfView(
               this.FieldOfView,
               this.AspectRatio,
               this.NearPlane,
               this.FarPlane);

            this.ComputeMatrices();
        }

        private void ComputeMatrices()
        {
            this.ViewProjection = this.View * this.Projection;
            this.Frustum = new BoundingFrustum(this.ViewProjection);
            this.InverseViewProjection = Matrix.Invert(this.ViewProjection);
        }       
    }
}