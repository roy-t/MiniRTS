using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Camera
{
    public interface ICamera
    {
        Matrix ViewProjection { get; }
        Matrix View { get; }
        Matrix Projection { get; }

        float AspectRatio { get; }
        float NearPlane { get; }
        float FarPlane { get; }

        Vector3 Position { get; }
        Vector3 Forward { get; }
    }
}
