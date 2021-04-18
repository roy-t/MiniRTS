using Microsoft.Xna.Framework;
using MiniEngine.Graphics.Camera;
using MiniEngine.Systems;

namespace MiniEngine.Graphics.Visibility
{
    public interface IRenderService
    {
        void DrawToGBuffer(PerspectiveCamera camera, Entity entity);
        void DrawToShadowMap(Matrix viewProjection, Entity entity);
    }
}
