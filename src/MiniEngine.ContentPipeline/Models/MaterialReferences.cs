using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.ContentPipeline.Models
{
    public sealed class MaterialReferences
    {
        public MaterialReferences(ExternalReference<Texture2D> albedo, ExternalReference<Texture2D> normal)
        {
            this.Albedo = albedo;
            this.Normal = normal;
        }

        public ExternalReference<Texture2D> Albedo { get; }
        public ExternalReference<Texture2D> Normal { get; }
    }
}
