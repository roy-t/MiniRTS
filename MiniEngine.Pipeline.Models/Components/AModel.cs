using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public abstract class AModel : IComponent
    {
        protected AModel(Model model, Pose pose, BoundingSphere boundingSphere, BoundingBox boundingBox)
        {
            this.Model = model;
            this.Pose = pose;
            this.BoundingSphere = boundingSphere;
            this.BoundingBox = boundingBox;
        }

        public Model Model { get; }
        public Pose Pose { get; set; }
        public BoundingSphere BoundingSphere { get; set; }
        public BoundingBox BoundingBox { get; set; }

        public abstract ComponentDescription Describe();

        protected void Describe(ComponentDescription description) 
            => description.AddProperty("Pose", this.Pose, x => this.Pose = x, MinMaxDescription.MinusInfinityToInfinity);       
    }
}
