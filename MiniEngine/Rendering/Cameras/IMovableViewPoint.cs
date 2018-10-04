using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Cameras
{
    public interface IMovableViewPoint : IViewPoint
    {
        void Move(Vector3 position, Vector3 lookAt);
    }
}