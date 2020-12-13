using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Camera
{
    public interface ICamera
    {
        Matrix ViewProjection { get; }

        Matrix View { get; }

        Matrix Projection { get; }

        Vector3 Position { get; }

        Vector3 Forward { get; }

        float AspectRatio { get; }

        void Move(Vector3 position, Vector3 forward);
    }
}
