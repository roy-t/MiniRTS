using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Debug.Factories
{
    public sealed class DebugInfoFactory : AComponentFactory<DebugInfo>
    {
        public DebugInfoFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity)
        {
            var outline = new DebugInfo(Color.Blue * 0.5f, Color.Red * 0.5f, Color.White, Color.TransparentBlack);
            this.Linker.AddComponent(entity, outline);
        }
    }
}
