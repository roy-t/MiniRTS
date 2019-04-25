using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Lights.Components
{
    [Label(nameof(AmbientLight))]
    public sealed class AmbientLight : IComponent
    {
        public AmbientLight(Color color)
        {
            this.Color = color;
        }

        [Editor(nameof(Color))]
        public Color Color { get; set; }        
    }
}
