using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Writers
{
    [ContentTypeWriter]
    internal sealed class GeometryModelWriter : ContentTypeWriter<GeometryModelContent>
    {
        public override string GetRuntimeReader(TargetPlatform targetPlatform)
         => typeof(GeometryModel).AssemblyQualifiedName;

        public override string GetRuntimeType(TargetPlatform targetPlatform)
            => typeof(GeometryModel).AssemblyQualifiedName;

        protected override void Write(ContentWriter output, GeometryModelContent value)
            => WriteGeometryModel(output, value);

        public static void WriteGeometryModel(ContentWriter output, GeometryModelContent value)
        {
            output.Write(value.Meshes.Count);
            foreach (var mesh in value.Meshes)
            {
                GeometryMeshWriter.WriteGeometryMesh(output, mesh);
            }
        }
    }
}
