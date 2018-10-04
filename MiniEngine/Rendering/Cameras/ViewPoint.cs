using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Cameras
{
    public sealed class ViewPoint : IViewPoint
    {
        public ViewPoint(Matrix view, Matrix projection, Vector3 position, Vector3 forward)
        {
            this.View = view;
            this.Projection = projection;
            this.Position = position;
            this.Forward = forward;
            this.Frustum = new BoundingFrustum(this.View * this.Projection);
        }

        public Matrix View { get; }
        public Matrix Projection { get; }
        public Vector3 Position { get; }
        public Vector3 Forward { get; }
        public BoundingFrustum Frustum { get; }
    }
}