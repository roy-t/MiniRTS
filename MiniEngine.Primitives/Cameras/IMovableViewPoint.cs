using Microsoft.Xna.Framework;

namespace MiniEngine.Primitives.Cameras
{
    public interface IMovableViewPoint : IViewPoint
    {
        void Move(Vector3 position, Vector3 lookAt);
    }
}