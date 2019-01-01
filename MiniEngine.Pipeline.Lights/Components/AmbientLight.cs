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

        public Color Color { get; private set; }

        public override string ToString() => $"ambient light, color: {this.Color}";
    }
}
