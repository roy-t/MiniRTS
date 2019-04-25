using Microsoft.Xna.Framework;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Debug.Components
{
    [Label(nameof(DebugInfo))]
    public sealed class DebugInfo : IComponent
    {        
        public DebugInfo(Color color3D, Color color2d)
        {
            this.Color3D = color3D;
            this.Color2D = color2d;
        }

        [Editor("Boundary 3D")]
        public Color Color3D { get; set; }

        [Editor("Bounary 2D")]
        public Color Color2D { get; set; }        
    }
}
