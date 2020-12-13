using System;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class CascadeCamera : IViewPoint
    {
        public CascadeCamera()
        {
            this.Frustum = new BoundingFrustum(Matrix.Identity);
        }

        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }

        public Matrix ViewProjection { get; private set; }

        public BoundingFrustum Frustum { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }

        public void CoverFrustum(Vector3 surfaceToLightVector, Frustum frustum, int resolution)
        {
            // By using a rounding radius, and offseting the view matrix to a rounded value
            // shimmering is reduced when the angle the shadow is seen at changes

            var bounds = frustum.ComputeBounds();
            var radius = (float)Math.Ceiling(bounds.Radius);

            this.Position = bounds.Center + (surfaceToLightVector * radius);
            this.Forward = Vector3.Normalize(bounds.Center - this.Position);

            this.View = Matrix.CreateLookAt(this.Position, bounds.Center, Vector3.Up);
            this.Projection = Matrix.CreateOrthographicOffCenter(
                -radius,
                radius,
                -radius,
                radius,
                0.0f,
                radius * 2);


            var origin = Vector3.Transform(Vector3.Zero, this.View * this.Projection);
            origin = origin * (resolution / 2.0f);

            var roundedOrigin = Round(origin);
            var roundOffset = roundedOrigin - origin;
            roundOffset = roundOffset * (2.0f / resolution);

            var projection = this.Projection;

            projection.M41 += roundOffset.X;
            projection.M42 += roundOffset.Y;

            this.Projection = projection;

            this.ViewProjection = this.View * this.Projection;
            this.Frustum.Matrix = this.ViewProjection;
        }

        private static Vector3 Round(Vector3 value)
        {
            return new Vector3(
                (float)Math.Round(value.X),
                (float)Math.Round(value.Y),
                (float)Math.Round(value.Z));
        }
    }
}
