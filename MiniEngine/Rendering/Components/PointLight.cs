using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Components
{
    public class PointLight
    {
        public PointLight(Vector3 position, Color color, float radius, float intensity)
        {
            this.Position = position;
            this.Color = color;
            this.Radius = radius;
            this.Intensity = intensity;
        }

        public Vector3 Position { get; set; }       
        public Color Color { get; set; }
        public float Radius { get; set; }
        public float Intensity { get; set; }
    }
}