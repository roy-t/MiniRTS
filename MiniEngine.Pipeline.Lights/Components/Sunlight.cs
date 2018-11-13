using Microsoft.Xna.Framework;

namespace MiniEngine.Pipeline.Lights.Components
{
    public sealed class Sunlight
    {
        public Sunlight(Color color, int cascades)
        {
            this.Color = color;
        }
        
        public Color Color { get; set; }        
    }
}