using Microsoft.Xna.Framework;

namespace MiniEngine.Primitives.Cameras
{
    public interface IViewPoint
    {
        Matrix View { get; }
        Matrix Projection { get; }
        BoundingFrustum Frustum { get; }

        Vector3 Position { get; }
        Vector3 Forward { get; }
    }
}