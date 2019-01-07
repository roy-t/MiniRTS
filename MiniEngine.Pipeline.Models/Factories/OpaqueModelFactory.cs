using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Bounds;
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
            var boundingSphere = model.ComputeBoundingSphere(pose.Matrix);
            var boundingBox = model.ComputeBoundingBox(pose.Matrix);

            var opaqueModel = new OpaqueModel(model, pose, boundingSphere, boundingBox);
            this.Linker.AddComponent(entity, opaqueModel);
        }
    }
}
