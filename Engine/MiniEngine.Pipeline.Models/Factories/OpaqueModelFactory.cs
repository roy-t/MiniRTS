using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class OpaqueModelFactory : AComponentFactory<OpaqueModel>
    {
        public OpaqueModelFactory(GraphicsDevice device, IComponentContainer<OpaqueModel> container)
            : base(device, container) { }

        public OpaqueModel Construct(Entity entity, Model model)
        {
            var opaqueModel = new OpaqueModel(entity, model);
            this.Container.Add(entity, opaqueModel);

            return opaqueModel;
        }
    }
}
