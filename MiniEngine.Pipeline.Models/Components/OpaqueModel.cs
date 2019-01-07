using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class OpaqueModel : AModel, IComponent
    {
        public OpaqueModel(Model model, Pose pose, BoundingSphere boundingSphere, BoundingBox boundingBox)
            : base(model, pose, boundingSphere, boundingBox) { }

        public ComponentDescription Describe()
        {
            var description = new ComponentDescription("Opaque model");
            this.Describe(description);

            return description;            
        }

        public override string ToString() 
            => $"opaque model, translation: {this.Pose.Translation}, yaw: {this.Pose.Yaw}, pitch: {this.Pose.Pitch}, roll: {this.Pose.Roll}, scale: {this.Pose.Scale}";
    }
}