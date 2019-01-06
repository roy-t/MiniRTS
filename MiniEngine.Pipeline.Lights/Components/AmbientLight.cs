using Microsoft.Xna.Framework;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class AmbientLight : IComponent
    {
        public AmbientLight(Color color)
        {
            this.Color = color;
        }

        public Color Color { get; set; }

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Ambient light");
            description.AddProperty("Color", this.Color, x => this.Color = x, 0.0f, 1.0f);
            return description;
        }


        public override string ToString() => $"ambient light, color: {this.Color}";
    }
}
