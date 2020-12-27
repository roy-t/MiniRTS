using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class ParticipatingMediaPostProcessEffect : EffectWrapper
    {
        private readonly EffectParameter MediaParameter;
        private readonly EffectParameter MediaColorParameter;

        public ParticipatingMediaPostProcessEffect(EffectFactory factory) : base(factory.Load<ParticipatingMediaPostProcessEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["ParticipatingMediaPostProcessTechnique"];

            this.MediaParameter = this.Effect.Parameters["Media"];
            this.MediaColorParameter = this.Effect.Parameters["MediaColor"];
        }

        public Texture2D Media { set => this.MediaParameter.SetValue(value); }

        public Color Color { set => this.MediaColorParameter.SetValue(value.ToVector3()); }
    }
}
