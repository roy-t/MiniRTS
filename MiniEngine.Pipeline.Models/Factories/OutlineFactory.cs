using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class OutlineFactory : AComponentFactory<Outline>
    {
        public OutlineFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity, AModel model)
        {
            var outline = new Outline(model, Color.Blue * 0.5f, Color.Red * 0.5f);
            this.Linker.AddComponent(entity, outline);
        }
    }
}
