using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Rendering.Components
{
    public sealed class ParticlePose
    {
        public ParticlePose(Vector2 minUv, Vector2 maxUv, Texture2D texture, Matrix pose)
        {
            this.MinUv = minUv;
            this.MaxUv = maxUv;
            this.Texture = texture;
            this.Pose = pose;
        }

        public Vector2 MinUv { get; }
        public Vector2 MaxUv { get; }
        public Texture2D Texture { get; }
        public Matrix Pose { get; }
    }
}
