using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class ParticipatingMediaPostProcessEffect : EffectWrapper
    {
        private readonly EffectParameter MediaParameter;
        private readonly EffectParameter MediaColorParameter;
        private readonly EffectParameter LightColorParameter;
        private readonly EffectParameter LightInfluenceParameter;
        private readonly EffectParameter DitherPatternParameter;
        private readonly EffectParameter ScreenDimensionsParameter;

        public ParticipatingMediaPostProcessEffect(EffectFactory factory) : base(factory.Load<ParticipatingMediaPostProcessEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticipatingMediaPostProcessTechnique"];

            this.MediaParameter = this.Effect.Parameters["Media"];
            this.MediaColorParameter = this.Effect.Parameters["MediaColor"];
            this.LightColorParameter = this.Effect.Parameters["LightColor"];
            this.LightInfluenceParameter = this.Effect.Parameters["LightInfluence"];
            this.DitherPatternParameter = this.Effect.Parameters["DitherPattern"];
            this.ScreenDimensionsParameter = this.Effect.Parameters["ScreenDimensions"];
        }

        public Texture2D Media { set => this.MediaParameter.SetValue(value); }

        public Color Color { set => this.MediaColorParameter.SetValue(value.ToVector3()); }

        public Color LightColor { set => this.LightColorParameter.SetValue(value.ToVector3()); }

        public float LightInfluence { set => this.LightInfluenceParameter.SetValue(value); }

        public Texture2D DitherPattern { set => this.DitherPatternParameter.SetValue(value); }

        public Vector2 ScreenDimensions { set => this.ScreenDimensionsParameter.SetValue(value); }
    }
}
