using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Debug.Components
{
    public sealed class DebugInfo : IComponent
    {
        public DebugInfo(Entity entity, IconType icon, Color color3D, Color color2d, Color boundaryVisibileTint, Color boundaryClippedTint)
        {
            this.Entity = entity;
            this.Icon = icon;
            this.Color3D = color3D;
            this.Color2D = color2d;

            this.BoundaryVisibleTint = boundaryVisibileTint;
            this.BoundaryClippedTint = boundaryClippedTint;
        }

        public Entity Entity { get; }

        public IconType Icon { get; }

        [Editor("Boundary 3D")]
        public Color Color3D { get; set; }

        [Editor("Bounary 2D")]
        public Color Color2D { get; set; }


        [Editor(nameof(BoundaryVisibleTint))]
        public Color BoundaryVisibleTint { get; set; }

        [Editor(nameof(BoundaryClippedTint))]
        public Color BoundaryClippedTint { get; set; }
    }
}
