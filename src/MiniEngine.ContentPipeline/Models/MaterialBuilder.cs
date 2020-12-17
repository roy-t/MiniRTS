using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

using M = MiniEngine.ContentPipeline.Serialization;

using X = Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace MiniEngine.ContentPipeline.Models
{
    internal sealed class MaterialBuilder
    {
        private readonly Dictionary<string, ExternalReference<Texture2D>> Cache;
        private readonly MaterialLookup LookUp;

        public MaterialBuilder(MaterialLookup lookup)
        {
            this.LookUp = lookup;
            this.Cache = new Dictionary<string, ExternalReference<Texture2D>>();
        }

        public M.MaterialContent Build(X.MaterialContent content, ContentProcessorContext context)
        {
            var albedo = this.LoadTexture(this.LookUp.GetAlbedo(content), context);
            var normal = this.LoadTexture(this.LookUp.GetAlbedo(content), context);

            return new M.MaterialContent(albedo, normal, 0.0f, 1.0f, 1.0f);
        }

        public ExternalReference<Texture2D> LoadTexture(ExternalReference<TextureContent> reference, ContentProcessorContext context)
        {
            if (!this.Cache.TryGetValue(reference.Filename, out var value))
            {
                value = context.BuildAsset<TextureContent, Texture2D>(reference, "TextureProcessor");

                var filename = reference.Filename;
                context.AddDependency(filename);
                this.Cache.Add(filename, value);
            }

            return value;
        }
    }
}
