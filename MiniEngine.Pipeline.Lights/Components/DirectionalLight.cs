using Microsoft.Xna.Framework;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class DirectionalLight
    {
        public DirectionalLight(Vector3 direction, Color color)
        {
            this.Direction = direction;
            this.Color = color;
        }

        public Vector3 Direction { get; set; }
        public Color Color { get; set; }
    }
}