using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Debug.Factories
{
    public sealed class DebugLineFactory : AComponentFactory<DebugLine>
    {
        public DebugLineFactory(GraphicsDevice device, IComponentContainer<DebugLine> container)
            : base(device, container) { }

        public DebugLine Construct(Entity entity, IReadOnlyList<Vector3> linePositions, Color color)
        {
            var debugLine = new DebugLine(entity, linePositions, color, Color.White, Color.Gray);
            this.Container.Add(entity, debugLine);

            return debugLine;
        }
    }
}
