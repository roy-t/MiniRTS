using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Components
{
    public sealed class ModelPose
    {
        public ModelPose(Model model, Matrix pose)
        {
            this.Model = model;
            this.Pose = pose;
        }

        public Model Model { get; }
        public Matrix Pose { get; set; }
    }
}
