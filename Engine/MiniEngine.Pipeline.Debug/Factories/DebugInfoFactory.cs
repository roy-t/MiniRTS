using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Debug.Factories
{
    public sealed class DebugInfoFactory : AComponentFactory<DebugInfo>
    {
        public DebugInfoFactory(GraphicsDevice device, IComponentContainer<DebugInfo> container)
            : base(device, container) { }

        public DebugInfo Construct(Entity entity, IconType icon)
        {
            var debugInfo = new DebugInfo(entity, icon, Color.Blue, Color.Red, Color.White, Color.FromNonPremultiplied(32, 32, 32, 255));
            this.Container.Add(debugInfo);

            return debugInfo;
        }
    }
}
