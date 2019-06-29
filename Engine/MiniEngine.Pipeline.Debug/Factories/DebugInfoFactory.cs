using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Debug.Factories
{
    public sealed class DebugInfoFactory : AComponentFactory<DebugInfo>
    {
        public DebugInfoFactory(GraphicsDevice device, IComponentContainer<DebugInfo> container) 
            : base(device, container) { }

        public DebugInfo Construct(Entity entity)
        {
            var debugInfo = new DebugInfo(entity, Color.Blue * 0.5f, Color.Red * 0.5f, Color.White, Color.TransparentBlack);
            this.Container.Add(debugInfo);

            return debugInfo;
        }
    }
}
