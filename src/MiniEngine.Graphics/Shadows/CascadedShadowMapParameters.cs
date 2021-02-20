using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Shadows
{
    public class CascadedShadowMapParameters
    {
        private readonly EffectParameter ShadowMapParameter;
        private readonly EffectParameter ShadowMatrixParameter;
        private readonly EffectParameter SplitsParameter;
        private readonly EffectParameter OffsetsParameter;
        private readonly EffectParameter ScalesParameter;

        public CascadedShadowMapParameters(Effect effect)
        {
            this.ShadowMapParameter = effect.Parameters["ShadowMap"];
            this.ShadowMatrixParameter = effect.Parameters["ShadowMatrix"];
            this.SplitsParameter = effect.Parameters["Splits"];
            this.OffsetsParameter = effect.Parameters["Offsets"];
            this.ScalesParameter = effect.Parameters["Scales"];

            this.ShadowMapSampler = new SamplerState
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = TextureFilter.Anisotropic,
                ComparisonFunction = CompareFunction.LessEqual,
                FilterMode = TextureFilterMode.Comparison
            };
        }

        public Texture2D ShadowMap { set => this.ShadowMapParameter.SetValue(value); }

        public Matrix ShadowMatrix { set => this.ShadowMatrixParameter.SetValue(value); }

        public float[] Splits { set => this.SplitsParameter.SetValue(value); }

        public Vector4[] Offsets { set => this.OffsetsParameter.SetValue(value); }

        public Vector4[] Scales { set => this.ScalesParameter.SetValue(value); }


        public SamplerState ShadowMapSampler { get; }
    }
}
