using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AModel
    {
        protected AModel(Model model, Matrix pose, BoundingSphere boundingSphere, BoundingBox boundingBox)
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


        protected void Describe(ComponentDescription description)
        {
            this.Pose.Decompose(out var scale, out var rotation, out var translation);

            // TODO: figure out how to rebuild a matrix correctly from the decomposed parts
            description.AddLabel("Translation", translation);
            description.AddLabel("Rotation", rotation);
            description.AddLabel("Scale", scale);
        }
    }
}
