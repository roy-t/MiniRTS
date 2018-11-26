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

        public override string ToString() => $"directional light, direction: {this.Direction}, color: {this.Color}";
    }
}