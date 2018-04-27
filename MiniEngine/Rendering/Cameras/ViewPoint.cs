using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Cameras
{
    public class ViewPoint : IViewPoint
    {
        public ViewPoint(Matrix view, Matrix projection)
        {
            this.View = view;
            this.Projection = projection;
        }

        public Matrix View { get; }        
        public Matrix Projection { get; }
    }
}