using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Debug.Components
{
    public sealed class DebugLine : IComponent
    {
        public DebugLine(Entity entity, IReadOnlyList<Vector3> positions, Color color, Color visibleTint, Color clippedTint)
        {
            this.Entity = entity;
            this.Positions = positions;
            this.Color = color;

            this.VisibleTint = visibleTint;
            this.ClippedTint = clippedTint;
        }

        public Entity Entity { get; }
        public IReadOnlyList<Vector3> Positions { get; set; }

        [Editor(nameof(Color))]
        public Color Color { get; set; }

        [Editor(nameof(VisibleTint))]
        public Color VisibleTint { get; set; }

        [Editor(nameof(ClippedTint))]
        public Color ClippedTint { get; set; }
    }
}
