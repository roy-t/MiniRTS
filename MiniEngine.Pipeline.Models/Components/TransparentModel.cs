using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class TransparentModel : AModel, IComponent
    {
        public TransparentModel(Model model, Pose pose, BoundingSphere boundingSphere, BoundingBox boundingBox)
            : base(model, pose, boundingSphere, boundingBox) { }

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Transparent model");
            this.Describe(description);

            return description;            
        }

        public override string ToString() 
            => $"transparent model, translation: {this.Pose.Translation}, yaw: {this.Pose.Yaw}, pitch: {this.Pose.Pitch}, roll: {this.Pose.Roll}, scale: {this.Pose.Scale}";
    }
}