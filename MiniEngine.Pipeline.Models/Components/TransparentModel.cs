using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Models.Components
{
    public sealed class TransparentModel : AModel,  IComponent
    {
        public TransparentModel(Model model, Matrix pose, BoundingSphere boundingSphere, BoundingBox boundingBox) 
            : base(model, pose, boundingSphere, boundingBox) { }

        public override string ToString()
        {
            this.Pose.Decompose(out var scale, out var rotation, out var translation);
            return $"opaque model, translation: {translation}, rotation: {rotation}, scale: {scale}";
        }
    }
}