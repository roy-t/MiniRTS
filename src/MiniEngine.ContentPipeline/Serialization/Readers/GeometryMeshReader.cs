using Microsoft.Xna.Framework.Content;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.ContentPipeline.Serialization.Readers
{
    internal sealed class GeometryMeshReader : ContentTypeReader<GeometryMesh>
    {
        protected override GeometryMesh Read(ContentReader input, GeometryMesh existingInstance)
            => Read(input);

        public static GeometryMesh Read(ContentReader input)
        {
            var geometry = GeometryDataReader.Read(input);
            var material = MaterialReader.Read(input);
            var offset = input.ReadMatrix();

            return new GeometryMesh(geometry, material, offset);
        }
    }
}
