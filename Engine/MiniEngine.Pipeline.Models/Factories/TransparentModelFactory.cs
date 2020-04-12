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

        public TransparentModel Construct(Entity entity, Model model)
        {
            var transparentModel = new TransparentModel(entity, model);
            this.Container.Add(entity, transparentModel);

            this.Bounds.Add(entity, new Bounds(entity, ModelBoundaryComputer.Compute(model)));

            return transparentModel;
        }
    }
}
