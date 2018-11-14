using Microsoft.Xna.Framework;
using MiniEngine.Pipeline.Shadows.Utilities;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using System;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class CascadeCamera : IViewPoint
    {               
        public Matrix View { get; private set; }
        
        private Matrix projection;
        public Matrix Projection => this.projection;

        public Matrix ViewProjection { get; private set; }

        public BoundingFrustum Frustum { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Forward { get; private set; }

        public CascadeCamera()
        {
            this.Frustum = new BoundingFrustum(Matrix.Identity);
        }

        public void CoverFrustum(Vector3 surfaceToLightVector, Frustum frustum, int shadowMapResolution)
        {
            var bounds = frustum.ComputeBounds();
            var radius = (float)Math.Ceiling(bounds.Radius);

            var position = bounds.Center + (surfaceToLightVector * radius);            
            this.Move(position, bounds.Center, radius, shadowMapResolution);
        }

        private void Move(Vector3 position, Vector3 lookAt, float radius, int resolution)
        {
            this.Position = position;
            this.Forward = Vector3.Normalize(lookAt - position);
            
            this.View = Matrix.CreateLookAt(position, lookAt, Vector3.Up);
            this.projection = Matrix.CreateOrthographicOffCenter(
                -radius,
                radius,
                -radius,
                radius,
                0.0f,
                radius * 2);
          
            var origin = Vector3.Transform(Vector3.Zero, this.View * this.Projection);
            origin = origin * (resolution / 2.0f);

            var roundedOrigin = origin.Round();
            var roundOffset = roundedOrigin - origin;
            roundOffset = roundOffset * (2.0f / resolution);
            roundOffset.Z = 0.0f;

            var projection = this.Projection;

            projection.M41 += roundOffset.X;
            projection.M42 += roundOffset.Y;
            projection.M43 += roundOffset.Z;

            this.projection = projection;
            
            this.ViewProjection = this.View * this.Projection;            
            this.Frustum.Matrix = this.ViewProjection;
        }
    }
}
