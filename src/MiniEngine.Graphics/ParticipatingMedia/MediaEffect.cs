using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class MediaEffect : EffectWrapper
    {
        private readonly EffectParameter WorldViewProjectionParameter;
        private readonly EffectParameter ChannelParameter;
        private readonly EffectParameter VolumeTextureParameter;

        public MediaEffect(EffectFactory factory) : base(factory.Load<MediaEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["VolumeMediaTechnique"];

            this.WorldViewProjectionParameter = this.Effect.Parameters["WorldViewProjection"];
            this.ChannelParameter = this.Effect.Parameters["Channel"];
            this.VolumeTextureParameter = this.Effect.Parameters["VolumeTexture"];
        }

        public Matrix WorldViewProjection { set => this.WorldViewProjectionParameter.SetValue(value); }

        public Vector2 Channel { set => this.ChannelParameter.SetValue(value); }

        public Texture2D VolumeTexture { set => this.VolumeTextureParameter.SetValue(value); }

        public void Apply(MediaTechnique technique)
        {
            switch (technique)
            {
                case MediaTechnique.Volume:
                    this.Effect.CurrentTechnique = this.Effect.Techniques["VolumeMediaTechnique"];
                    break;
                case MediaTechnique.Density:
                    this.Effect.CurrentTechnique = this.Effect.Techniques["DensityMediaTechnique"];
                    break;
            }

            this.Apply();
        }
    }

    public enum MediaTechnique
    {
        Volume,
        Density
    }
}
