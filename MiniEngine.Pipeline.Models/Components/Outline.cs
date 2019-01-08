using Microsoft.Xna.Framework;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class Outline : IComponent
    {
        public Outline(AModel model, Color color3D, Color color2d)
        {
            this.Model = model;
            this.Color3D = color3D;
            this.Color2D = color2d;
        }

        public AModel Model { get; }
        public Color Color3D { get; set; }
        public Color Color2D { get; set; }

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Outline");
            description.AddProperty("Color 3D", this.Color3D, x => this.Color3D = x);
            description.AddProperty("Color 2D", this.Color2D, x => this.Color2D = x);

            return description;
        }

        public override string ToString()
            => $"outline, 3D: {this.Color3D}, 2D: {this.Color2D}";
    }
}
