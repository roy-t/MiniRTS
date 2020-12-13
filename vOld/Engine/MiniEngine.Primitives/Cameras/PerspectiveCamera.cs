using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives.Cameras
{
    public sealed class PerspectiveCamera : IViewPoint
    {
        private const float Epsilon = 0.001f;

        public PerspectiveCamera(int width, int height)
            : this(new Viewport(0, 0, width, height)) { }

        public PerspectiveCamera(Viewport viewport)
        {
            this.NearPlane = 0.1f;
            this.FarPlane = 250.0f;
            this.Viewport = viewport;

            this.Move(Vector3.Backward * 10, Vector3.Zero);
            this.SetFieldOfView(MathHelper.PiOver2);
        }

        public Viewport Viewport { get; }

        public float NearPlane { get; private set; }
        public float FarPlane { get; private set; }
        public float AspectRatio => this.Viewport.AspectRatio;
        public float FieldOfView { get; private set; }
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

        public void SetPlanes(float near, float far)
        {
            this.NearPlane = MathHelper.Clamp(near, Epsilon, float.MaxValue);
            this.FarPlane = MathHelper.Clamp(far, this.NearPlane + Epsilon, float.MaxValue);

            this.Projection = Matrix.CreatePerspectiveFieldOfView(
              this.FieldOfView,
              this.AspectRatio,
              this.NearPlane,
              this.FarPlane);

            this.ComputeMatrices();
        }

        public Vector3? Pick(Point position, float depth)
        {
            var near = this.Viewport.Unproject(new Vector3(position.X, position.Y, 0), this.Projection, this.View, Matrix.Identity);
            var far = this.Viewport.Unproject(new Vector3(position.X, position.Y, 1), this.Projection, this.View, Matrix.Identity);

            var direction = Vector3.Normalize(far - near);
            var ray = new Ray(near, direction);

            var plane = new Plane(Vector3.Up, depth);
            var intersection = ray.Intersects(plane);
            if (!intersection.HasValue)
            {
                return null;
            }
            else
            {
                return near + (direction * intersection.Value);
            }
        }

        private void ComputeMatrices()
        {
            this.ViewProjection = this.View * this.Projection;
            this.Frustum = new BoundingFrustum(this.ViewProjection);
            this.InverseViewProjection = Matrix.Invert(this.ViewProjection);
        }
    }
}