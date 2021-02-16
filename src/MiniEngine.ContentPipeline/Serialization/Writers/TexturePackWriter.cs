using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiniEngine.ContentPipeline.Packs;
using MiniEngine.ContentPipeline.Serialization.Readers;

namespace MiniEngine.ContentPipeline.Serialization.Writer
{
    [ContentTypeWriter]
    internal sealed class TexturePackWriter : ContentTypeWriter<TexturePack>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
            => typeof(TexturePackReader).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, TexturePack value)
        {
            output.Write(value.Name);
            output.Write(value.Names.Count);
            foreach (var texture in value.Names)
            {
                output.Write(texture);
            }
        }
    }
}
