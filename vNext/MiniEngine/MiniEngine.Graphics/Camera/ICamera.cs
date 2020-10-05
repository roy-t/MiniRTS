using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Camera
{
    public interface ICamera
    {
        Matrix ViewProjection { get; }

        Vector3 Position { get; }

        Vector3 Forward { get; }

        void Move(Vector3 position, Vector3 forward);
    }
}
