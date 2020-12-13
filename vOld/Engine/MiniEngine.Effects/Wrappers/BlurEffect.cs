using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Effects.Wrappers
{
    public sealed class BlurEffect : EffectWrapper
    {
        public BlurEffect()
        {
        }

        public BlurEffect(Effect blurEffect)
        {
            this.Wrap(blurEffect);
        }
      
        /// <summary>
        /// Texture to blur
        /// </summary>
        public Texture2D SourceMap
        {
            set => this.effect.Parameters["SourceMap"].SetValue(value);
        }

        /// <summary>
        /// Depth map used to determine which pixels are parts of the same object and can be blurred together
        /// this prevents blur bleeding over edges and between objects and such
        /// </summary>
        public Texture2D DepthMap
        {
            set => this.effect.Parameters["DepthMap"].SetValue(value);
        }

        /// <summary>
        /// Max distance in the depth map, usually the value of the camera's far plane
        /// </summary>
        public float MaxDistance
        {
            set => this.effect.Parameters["MaxDistance"].SetValue(value);
        }

        /// <summary>
        /// Max z-distance between two pixels to consider them a member of the same
        /// object and apply blur to them
        /// </summary>
        public float MaxBlurDistance
        {
            set => this.effect.Parameters["MaxBlurDistance"].SetValue(value);
        }

        /// <summary>
        /// Radius of the blur effect, as a percentage of the complete texture
        /// </summary>
        public float SampleRange
        {
            set => this.effect.Parameters["SampleRadius"].SetValue(value);
        }

        public void Apply() => this.ApplyPass();
    }
}