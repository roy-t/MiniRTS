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

        public void Construct(Entity entity, Model model, Pose pose)
            => this.Construct(entity, model, pose, new IdentityAnimation());

        public void Construct(Entity entity, Model model, Pose pose, AAnimation animation)
        {
            var transparentModel = new TransparentModel(entity, model, pose);

            animation.SetTarget(transparentModel);
            transparentModel.Animation = animation;

            this.Container.Add(transparentModel);
        }
    }
}
