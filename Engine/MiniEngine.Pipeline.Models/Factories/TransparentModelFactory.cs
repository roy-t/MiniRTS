using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Components;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class TransparentModelFactory : AComponentFactory<TransparentModel>
    {
        private readonly IComponentContainer<Bounds> Bounds;

        public TransparentModelFactory(GraphicsDevice device,
            IComponentContainer<TransparentModel> container,
            IComponentContainer<Bounds> bounds)
            : base(device, container)
        {
            this.Bounds = bounds;
        }

        public (TransparentModel, Bounds) Construct(Entity entity, Model model)
        {
            var transparentModel = new TransparentModel(entity, model);
            this.Container.Add(transparentModel);

            BoundaryComputer.ComputeExtremes(model, out var min, out var max);
            var bounds = new Bounds(entity, min, max);
            this.Bounds.Add(bounds);

            return (transparentModel, bounds);
        }

        public override void Deconstruct(Entity entity)
        {
            this.Bounds.Remove(entity);
            base.Deconstruct(entity);
        }
    }
}
