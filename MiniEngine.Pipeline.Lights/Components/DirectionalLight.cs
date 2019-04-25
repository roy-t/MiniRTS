using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    [Label(nameof(DirectionalLight))]
    public sealed class DirectionalLight : IComponent
    {
        public DirectionalLight(Vector3 direction, Color color)
        {
            this.Direction = direction;
            this.Color = color;
        }

        [Editor(nameof(Direction), nameof(Direction), -1, 1)]
        public Vector3 Direction { get; set; }

        [Editor(nameof(Color), nameof(Color))]
        public Color Color { get; set; }        
    }
}