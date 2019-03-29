using Microsoft.Xna.Framework;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class PointLight : IComponent
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

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Point light");
            description.AddProperty("Color", this.Color, x => this.Color = x, MinMaxDescription.ZeroToOne);
            description.AddProperty("Position", this.Position, x => this.Position = x, MinMaxDescription.MinusInfinityToInfinity);
            description.AddProperty("Radius", this.Radius, x => this.Radius = x, MinMaxDescription.ZeroToInfinity);
            description.AddProperty("Intensity", this.Intensity, x => this.Intensity = x, MinMaxDescription.ZeroToOne);

            return description;
        }

        public override string ToString() => $"point light, position: {this.Position}, color: {this.Color}, radius: {this.Radius}";
    }
}