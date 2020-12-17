using Microsoft.Xna.Framework;

namespace MiniEngine.ContentPipeline.Serialization
{
    internal sealed class GeometryMeshContent
    {
        public GeometryMeshContent(GeometryDataContent geometryData, MaterialContent material, Matrix offset)
        {
            this.GeometryData = geometryData;
            this.Material = material;
            this.Offset = offset;
        }

        public GeometryDataContent GeometryData { get; }

        public MaterialContent Material { get; }

        public Matrix Offset { get; }
    }
}
