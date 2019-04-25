using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Bounds;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AModel : IComponent
    {
        protected AModel(Model model, Pose pose)
        {
            this.Model = model;
            this.SetPose(pose);
        }

        public Model Model { get; }

        [Editor(nameof(Pose), nameof(SetPose))]
        public Pose Pose { get; private set; }

        public BoundingSphere BoundingSphere { get; private set; }

        [Boundary(BoundaryType.BoundingBox)]
        public BoundingBox BoundingBox { get; private set; }           

        public void SetPose(Pose pose)
        {
            this.Pose           = pose;
            this.BoundingSphere = this.Model.ComputeBoundingSphere(pose.Matrix);
            this.BoundingBox    = this.Model.ComputeBoundingBox(pose.Matrix);
        }
    }
}
