using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Cameras
{
    public interface IViewPoint
    {
        Matrix View { get; }
        Matrix Projection { get; }
        BoundingFrustum Frustum { get; }
    }
}
