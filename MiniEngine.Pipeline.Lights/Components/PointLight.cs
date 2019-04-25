using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    [Label(nameof(PointLight))]
    public sealed class PointLight : IComponent
    {
        public PointLight(Vector3 position, Color color, float radius, float intensity)
        {
            this.Position = position;
            this.Color = color;
            this.Radius = radius;
            this.Intensity = intensity;
        }

        [Editor(nameof(Position), nameof(Position))]
        public Vector3 Position { get; set; }

        [Editor(nameof(Color), nameof(Color))]
        public Color Color { get; set; }

        [Editor(nameof(Radius), nameof(Radius), 0, float.MaxValue)]
        public float Radius { get; set; }

        [Editor(nameof(Intensity), nameof(Intensity), 0, float.MaxValue)]
        public float Intensity { get; set; }        
    }
}