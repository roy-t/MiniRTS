using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class TransparentModelFactory : AComponentFactory<TransparentModel>
    {
        public TransparentModelFactory(GraphicsDevice device, IComponentContainer<TransparentModel> container)
            : base(device, container) { }

        public TransparentModel Construct(Entity entity, Model model, Pose pose)
        {
            var transparentModel = new TransparentModel(entity, model, pose);
            this.Container.Add(transparentModel);

            return transparentModel;
        }
    }
}
