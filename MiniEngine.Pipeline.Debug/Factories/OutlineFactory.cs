using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Debug.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Debug.Factories
{
    public sealed class OutlineFactory : AComponentFactory<Outline>
    {
        public OutlineFactory(GraphicsDevice device, EntityLinker linker) 
            : base(device, linker) { }

        public void Construct(Entity entity)
        {
            var model = this.Linker.GetComponent<AModel>(entity);

            var outline = new Outline(model, Color.Blue * 0.5f, Color.Red * 0.5f);
            this.Linker.AddComponent(entity, outline);
        }
    }
}
