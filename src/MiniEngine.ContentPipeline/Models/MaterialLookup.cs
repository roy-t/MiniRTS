using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.Models
{
    public class MaterialLookup
    {
        private readonly string FallbackAlbedo;
        private readonly string FallbackMetalicness;
        private readonly string FallbackNormal;
        private readonly string FallbackRoughness;
        private readonly string FallbackAmbientOcclusion;

        public MaterialLookup(string fallbackAlbedo, string fallbackMetalicness, string fallbackNormal, string fallbackRoughness, string fallbackAmbientOcclusion)
        {
            this.FallbackAlbedo = Path.GetFullPath(fallbackAlbedo);
            this.FallbackMetalicness = Path.GetFullPath(fallbackMetalicness);
            this.FallbackNormal = Path.GetFullPath(fallbackNormal);
            this.FallbackRoughness = Path.GetFullPath(fallbackRoughness);
            this.FallbackAmbientOcclusion = Path.GetFullPath(fallbackAmbientOcclusion);
        }

        public ExternalReference<TextureContent> GetAlbedo(MaterialContent material)
            => GetBestMatch(material, this.FallbackAlbedo, AlbedoTextureNames);

        public ExternalReference<TextureContent> GetMetalicness(MaterialContent material)
            => GetBestMatch(material, this.FallbackMetalicness, MetalicnessTextureNames);

        public ExternalReference<TextureContent> GetNormal(MaterialContent material)
            => GetBestMatch(material, this.FallbackNormal, NormalTextureNames);

        public ExternalReference<TextureContent> GetRoughness(MaterialContent material)
            => GetBestMatch(material, this.FallbackRoughness, RoughnessTextureNames);

        public ExternalReference<TextureContent> GetAmbientOcclusion(MaterialContent material)
            => GetBestMatch(material, this.FallbackAmbientOcclusion, AmbientOcclusionTextureNames);

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

        private static string[] AmbientOcclusionTextureNames => new[]
       {
            "AmbientOcclusion",
        };
    }
}
