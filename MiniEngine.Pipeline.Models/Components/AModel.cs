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
            description.AddProperty("Translation", translation, x => this.Pose = CreatePose(scale, rotation, x), -100.0f, 100.0f);
            description.AddProperty("Scale", scale, x => this.Pose = CreatePose(x, rotation, translation), 0.001f, 2.0f);
            description.AddProperty("Rotation", rotation, x => this.Pose = CreatePose(scale, x, translation), -MathHelper.Pi, MathHelper.Pi);
        }


        private static Matrix CreatePose(Vector3 scale, Quaternion rotation, Vector3 translation)
        {
            var r = Matrix.CreateFromQuaternion(rotation);
            var s = Matrix.CreateScale(scale);
            var t = Matrix.CreateTranslation(translation);

            return s * r * t;
        }
    }
}
