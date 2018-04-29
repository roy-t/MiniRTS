using Microsoft.Xna.Framework;

namespace MiniEngine.Rendering.Lighting.Components
{
    public class PointLight
    {        
        public PointLight(Vector3 position, Color color, float radius, float intensity)
        {
            this.Position = position;
            this.ColorVector = color.ToVector3();
            this.Radius = radius;
            this.Intensity = intensity;
        }

        public Vector3 Position { get; set; }

        public Vector3 ColorVector { get; set; }

        public Color Color
        {
            get => new Color(this.ColorVector);
            set => this.ColorVector = value.ToVector3();
        }

        public float Radius { get; set; }
        public float Intensity { get; set; }
    }
}
