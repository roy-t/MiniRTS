using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using MiniEngine.Systems.Factories;

namespace MiniEngine.Pipeline.Models.Factories
{
    public sealed class OpaqueModelFactory : AComponentFactory<OpaqueModel>
    {
        public OpaqueModelFactory(GraphicsDevice device, IComponentContainer<OpaqueModel> container)
            : base(device, container) { }

        public OpaqueModel Construct(Entity entity, Model model, Pose pose)
            => this.Construct(entity, model, pose, new IdentityAnimation());

        public OpaqueModel Construct(Entity entity, Model model, Pose pose, AAnimation animation)
        {
            var opaqueModel = new OpaqueModel(entity, model, pose);

            animation.SetTarget(opaqueModel);
            opaqueModel.Animation = animation;

            this.Container.Add(opaqueModel);

            return opaqueModel;
        }
    }
}
