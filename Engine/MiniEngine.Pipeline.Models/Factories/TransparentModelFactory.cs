using Microsoft.Xna.Framework;
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

            ModelBoundaryComputer.ComputeExtremes(model, Matrix.Identity, out var min, out var max);
            this.Bounds.Add(entity, new Bounds(entity, min, max));

            return transparentModel;
        }
    }
}
