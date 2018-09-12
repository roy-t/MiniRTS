using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Components
{
    public sealed class ModelPose
    {
        public ModelPose(Model model, Matrix pose, BoundingSphere bounds)
        {
            this.Model = model;
            this.Pose = pose;
            this.Bounds = bounds;
        }

        public Model Model { get; }
        public Matrix Pose { get; set; }
        public BoundingSphere Bounds { get; set; }
    }
}
