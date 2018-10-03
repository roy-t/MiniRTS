using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Primitives;

namespace MiniEngine.Rendering.Components
{
    public sealed class ParticlePose
    {
        public ParticlePose(Quad quad, Texture2D texture, Matrix pose)
        {
            this.Quad = quad;
            this.Texture = texture;
            this.Pose = pose;
        }

        public Quad Quad { get; }
        public Texture2D Texture { get; }
        public Matrix Pose { get; }
    }
}
