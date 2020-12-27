using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Shadows
{
    public sealed class VolumeEffect : EffectWrapper
    {
        private readonly EffectParameter VolumeFrontParameter;
        private readonly EffectParameter VolumeBackParameter;

        public VolumeEffect(EffectFactory factory) : base(factory.Load<VolumeEffect>())
        {
            this.Effect.CurrentTechnique = this.Effect.Techniques["VolumeTechnique"];

            this.VolumeFrontParameter = this.Effect.Parameters["VolumeFront"];
            this.VolumeBackParameter = this.Effect.Parameters["VolumeBack"];
        }

        public Texture2D VolumeFront { set => this.VolumeFrontParameter.SetValue(value); }

        public Texture2D VolumeBack { set => this.VolumeBackParameter.SetValue(value); }
    }
}
