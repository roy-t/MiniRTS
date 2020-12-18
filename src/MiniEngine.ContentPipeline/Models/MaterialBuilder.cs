using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

using M = MiniEngine.ContentPipeline.Serialization;

using X = Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.Models
{
    internal sealed class MaterialBuilder
    {
        private readonly Dictionary<string, ExternalReference<TextureContent>> Cache;
        private readonly MaterialLookup LookUp;

        private OpaqueDataDictionary AlbedoParameters = new OpaqueDataDictionary
        {
            { "ColorKeyColor", Color.Magenta },
            { "ColorKeyEnabled", false },
            { "GenerateMipmaps", true },
            { "PremultiplyAlpha", true },
            { "ResizeToPowerOfTwo", false },
            { "TextureFormat", TextureProcessorOutputFormat.Color }
        };

        private OpaqueDataDictionary Parameters = new OpaqueDataDictionary
        {
            { "ColorKeyColor", Color.Magenta },
            { "ColorKeyEnabled", false },
            { "GenerateMipmaps", true },
            { "PremultiplyAlpha", false },
            { "ResizeToPowerOfTwo", false },
            { "TextureFormat", TextureProcessorOutputFormat.Color16Bit }
        };

        public MaterialBuilder(MaterialLookup lookup)
        {
            this.LookUp = lookup;
            this.Cache = new Dictionary<string, ExternalReference<TextureContent>>();
        }

        public M.MaterialContent Build(X.MaterialContent content, ContentProcessorContext context)
        {
            var albedo = this.LoadTexture(this.AlbedoParameters, this.LookUp.GetAlbedo(content), context);
            var normal = this.LoadTexture(this.Parameters, this.LookUp.GetNormal(content), context);

            return new M.MaterialContent(albedo, normal, 0.0f, 1.0f, 1.0f);
        }

        public ExternalReference<TextureContent> LoadTexture(OpaqueDataDictionary parameters, ExternalReference<TextureContent> reference, ContentProcessorContext context)
        {
            if (!this.Cache.TryGetValue(reference.Filename, out var value))
            {
                value = BuildTexture(parameters, reference, context);

                var filename = reference.Filename;
                context.AddDependency(filename);
                this.Cache.Add(filename, value);
            }

            return value;
        }

        private ExternalReference<TextureContent> BuildTexture(OpaqueDataDictionary parameters, ExternalReference<TextureContent> texture, ContentProcessorContext context)
            => context.BuildAsset<TextureContent, TextureContent>(texture, "TextureProcessor", parameters, "TextureImporter", null);
    }
}
