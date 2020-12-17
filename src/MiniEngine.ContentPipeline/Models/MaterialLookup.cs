using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.Models
{
    public class MaterialLookup
    {
        public readonly string FallbackAlbedo;
        public readonly string FallbackMetalicness;
        public readonly string FallbackNormal;
        public readonly string FallbackRoughness;
        public readonly string FallbackMask;

        public MaterialLookup(string fallbackAlbedo, string fallbackMetalicness, string fallbackNormal, string fallbackRoughness, string fallbackMask)
        {
            this.FallbackAlbedo = Path.GetFullPath(fallbackAlbedo);
            this.FallbackMetalicness = Path.GetFullPath(fallbackMetalicness);
            this.FallbackNormal = Path.GetFullPath(fallbackNormal);
            this.FallbackRoughness = Path.GetFullPath(fallbackRoughness);
            this.FallbackMask = Path.GetFullPath(fallbackMask);
        }

        public ExternalReference<TextureContent> GetAlbedo(MaterialContent material)
            => GetBestMatch(material, this.FallbackAlbedo, AlbedoTextureNames);

        public ExternalReference<TextureContent> GetMetalicness(MaterialContent material)
            => GetBestMatch(material, this.FallbackMetalicness, MetalicnessTextureNames);

        public ExternalReference<TextureContent> GetNormal(MaterialContent material)
            => GetBestMatch(material, this.FallbackNormal, NormalTextureNames);

        public ExternalReference<TextureContent> GetRoughness(MaterialContent material)
            => GetBestMatch(material, this.FallbackRoughness, RoughnessTextureNames);

        public ExternalReference<TextureContent> GetMask(MaterialContent material)
            => GetBestMatch(material, this.FallbackMask, MaskTextureNames);

        private static ExternalReference<TextureContent> GetBestMatch(MaterialContent material, string fallback, string[] keys)
        {
            foreach (var key in keys)
            {
                if (material.Textures.ContainsKey(key))
                {
                    return material.Textures[key];
                }
            }

            return new ExternalReference<TextureContent>(fallback, material.Identity);
        }

        private static string[] AlbedoTextureNames => new[]
        {
            "Albedo",
            "Texture",
            "Diffuse"
        };

        private static string[] MetalicnessTextureNames => new[]
        {
            "Metalicness",
            "Ambient"
        };

        private static string[] NormalTextureNames => new[]
        {
            "Normal",
            "Height",
            "Bump"
        };

        private static string[] RoughnessTextureNames => new[]
        {
            "Roughness",
            "Shininess"
        };

        private static string[] MaskTextureNames => new[]
        {
            "Mask",
            "Opacity"
        };
    }
}
