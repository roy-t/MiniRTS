using Microsoft.Xna.Framework;

namespace MiniEngine.Graphics.Camera
{
    public interface ICamera
    {
        Matrix ViewProjection { get; }
    }
}
