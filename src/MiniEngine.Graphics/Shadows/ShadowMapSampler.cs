using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Shadows
{
    public static class ShadowMapSampler
    {
        public static SamplerState State { get; } = new SamplerState
        {
            AddressU = TextureAddressMode.Clamp,
            AddressV = TextureAddressMode.Clamp,
            AddressW = TextureAddressMode.Clamp,
            Filter = TextureFilter.Anisotropic,
            ComparisonFunction = CompareFunction.LessEqual,
            FilterMode = TextureFilterMode.Comparison
        };
    }
}
