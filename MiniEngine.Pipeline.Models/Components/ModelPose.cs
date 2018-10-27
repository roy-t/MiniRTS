using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class ModelPose
    {
        public ModelPose(Model model, Matrix pose, BoundingSphere boundingSphere, BoundingBox boundingBox)
        {
            this.Model = model;
            this.Pose = pose;
            this.BoundingSphere = boundingSphere;
            this.BoundingBox = boundingBox;
        }

        public Model Model { get; }
        public Matrix Pose { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
        public BoundingBox BoundingBox { get; set; }
    }
}