using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiniEngine.ContentPipeline.Serialization.Readers;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Writers
{
    [ContentTypeWriter]
    internal sealed class GeometryMeshWriter : ContentTypeWriter<GeometryMeshContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
          => typeof(GeometryMeshReader).AssemblyQualifiedName;

        public override string GetRuntimeType(TargetPlatform targetPlatform)
            => typeof(GeometryMesh).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, GeometryMeshContent value)
            => WriteGeometryMesh(output, value);

        public static void WriteGeometryMesh(ContentWriter output, GeometryMeshContent value)
        {
            GeometryDataWriter.WriteGeometryData(output, value.GeometryData);
            MaterialWriter.WriteMaterial(output, value.Material);

            output.Write(value.Offset);
        }
    }
}
