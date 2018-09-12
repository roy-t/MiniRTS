using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Cameras
{
    public sealed class ViewPoint : IViewPoint
    {
        public ViewPoint(Matrix view, Matrix projection)
        {
            this.View = view;
            this.Projection = projection;
            this.Frustum = new BoundingFrustum(this.View * this.Projection);
        }

        public Matrix View { get; }        
        public Matrix Projection { get; }
        public BoundingFrustum Frustum { get; }
    }
}