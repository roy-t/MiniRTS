using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class Sunlight : IViewPoint
    {
        private const int ShadowMapResolution = 1024;

        // Sunlight based on tutorial from
        // http://dev.theomader.com/cascaded-shadow-mapping-1/

        private readonly float[] ViewSpaceSplitDistances = {
            -1,
            -100.0f,
            -300.0f,
            -600.0f
        };

        private readonly BoundingBox SceneBoundingBox;
        private readonly BoundingSphere SceneBoundingSphere;

        public Sunlight(GraphicsDevice device, BoundingBox sceneBoundingBox, BoundingSphere sceneBoundingSphere, Camera camera)
        {
            this.ShadowMap = new RenderTarget2D(device, ShadowMapResolution, ShadowMapResolution, false, SurfaceFormat.Single, DepthFormat.Depth24, 0, RenderTargetUsage.DiscardContents);

            this.SceneBoundingBox = sceneBoundingBox;
            this.SceneBoundingSphere = sceneBoundingSphere;                     

            Move(Vector3.Backward * 10, Vector3.Zero, camera);
        }

        public RenderTarget2D ShadowMap { get; }

        public FrustumSplitProjection[] FrustumSplitProjections { get; private set; }
        
        public Vector3 Position { get; private set; }
        public Vector3 LookAt { get; private set; }
        public Matrix View { get; private set; }
        public Matrix Projection { get; private set; }
        public Matrix Transform { get; private set; }

        public void Move(Vector3 position, Vector3 lookAt, Camera camera)
        {
            this.Position = position;
            this.LookAt = lookAt;

            this.View = Matrix.CreateLookAt(position, lookAt, Vector3.Up);

            var center = Vector3.Transform(this.SceneBoundingSphere.Center, this.View);
            var min = center - new Vector3(this.SceneBoundingSphere.Radius);
            var max = center + new Vector3(this.SceneBoundingSphere.Radius);
            
            this.Projection = Matrix.CreateOrthographicOffCenter(min.X, max.X, min.Y, max.Y, -max.Z, -min.Z);
            this.Transform = this.View * this.Projection;

            this.FrustumSplitProjections = Frustum.SplitFrustum(this, camera, this.SceneBoundingBox, this.ViewSpaceSplitDistances);
        }        
    }
}
