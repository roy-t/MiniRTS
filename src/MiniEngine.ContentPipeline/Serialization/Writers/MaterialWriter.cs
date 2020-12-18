using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiniEngine.ContentPipeline.Serialization.Readers;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Writers
{
    [ContentTypeWriter]
    internal sealed class MaterialWriter : ContentTypeWriter<MaterialContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
           => typeof(MaterialReader).AssemblyQualifiedName;

        public override string GetRuntimeType(TargetPlatform targetPlatform)
            => typeof(Material).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, MaterialContent value)
            => WriteMaterial(output, value);

        public static void WriteMaterial(ContentWriter output, MaterialContent value)
        {
            output.WriteExternalReference(value.Albedo);
            output.WriteExternalReference(value.Normal);
            output.WriteExternalReference(value.Metalicness);
            output.WriteExternalReference(value.Roughness);
            output.WriteExternalReference(value.AmbientOcclusion);
        }
    }
}
