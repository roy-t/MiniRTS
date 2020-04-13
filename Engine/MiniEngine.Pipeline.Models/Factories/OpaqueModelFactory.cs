using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class OpaqueModelFactory : AComponentFactory<OpaqueModel>
    {
        private readonly IComponentContainer<Bounds> Bounds;

        public OpaqueModelFactory(GraphicsDevice device,
            IComponentContainer<OpaqueModel> container,
            IComponentContainer<Bounds> bounds)
            : base(device, container)
        {
            this.Bounds = bounds;
        }

        public OpaqueModel Construct(Entity entity, Model model)
        {
            var opaqueModel = new OpaqueModel(entity, model);
            this.Container.Add(entity, opaqueModel);

            ModelBoundaryComputer.ComputeExtremes(model, Matrix.Identity, out var min, out var max);
            this.Bounds.Add(entity, new Bounds(entity, min, max));

            return opaqueModel;
        }

        public override void Deconstruct(Entity entity)
        {
            this.Bounds.Remove(entity);
            base.Deconstruct(entity);
        }
    }
}
