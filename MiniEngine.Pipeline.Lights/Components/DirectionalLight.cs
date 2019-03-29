using Microsoft.Xna.Framework;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class DirectionalLight : IComponent
    {
        public DirectionalLight(Vector3 direction, Color color)
        {
            this.Direction = direction;
            this.Color = color;
        }

        public Vector3 Direction { get; set; }
        public Color Color { get; set; }

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Directional light");
            description.AddProperty("Direction", this.Direction, x => this.Direction = x, MinMaxDescription.MinusOneToOne);
            description.AddProperty("Color", this.Color, x => this.Color = x, MinMaxDescription.ZeroToOne);
            return description;
        }

        public override string ToString() => $"directional light, direction: {this.Direction}, color: {this.Color}";
    }
}