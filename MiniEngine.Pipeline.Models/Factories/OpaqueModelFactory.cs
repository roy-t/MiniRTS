using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class OpaqueModelFactory : AComponentFactory<OpaqueModel>
    {
        public OpaqueModelFactory(GraphicsDevice device, EntityLinker linker)
            : base(device, linker) { }

        public void Construct(Entity entity, Model model, Pose pose)
        {           
            var opaqueModel = new OpaqueModel(model, pose);
            this.Linker.AddComponent(entity, opaqueModel);
        }
    }
}
