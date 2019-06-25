using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Debug.Components
{
    public sealed class DebugInfo : IComponent
    {        
        public DebugInfo(Entity entity, Color color3D, Color color2d, Color visibleIconTint, Color clippedIconTint)
        {
            this.Entity = entity;
            this.Color3D = color3D;
            this.Color2D = color2d;

            this.VisibileIconTint = visibleIconTint;
            this.ClippedIconTint  = clippedIconTint;
        }

        public Entity Entity { get; }

        [Editor("Boundary 3D")]
        public Color Color3D { get; set; }

        [Editor("Bounary 2D")]
        public Color Color2D { get; set; }


        [Editor(nameof(VisibileIconTint))]
        public Color VisibileIconTint { get; set; }

        [Editor(nameof(ClippedIconTint))]
        public Color ClippedIconTint { get; set; }
    }
}
