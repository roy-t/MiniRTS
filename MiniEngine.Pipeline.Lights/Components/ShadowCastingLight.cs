using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class ShadowCastingLight
    {
        private const int ShadowMapResolution = 1024;

        public ShadowCastingLight(Vector3 position, Vector3 lookAt, Color color)
        {
            this.Color = color;
            this.ViewPoint = new PerspectiveCamera(new Viewport(0, 0, ShadowMapResolution, ShadowMapResolution));
            this.ViewPoint.Move(position, lookAt);
        }

        public PerspectiveCamera ViewPoint { get; }
        public Color Color { get; set; }
    }
}