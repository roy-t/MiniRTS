using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering
{
    public interface IViewPoint
    {
        Matrix View { get; }
        Matrix Projection { get; }
    }
}
