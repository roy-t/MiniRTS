using System.Collections.Generic;

namespace MiniEngine.ContentPipeline.Serialization
{
    internal sealed class GeometryModelContent
    {
        private readonly List<GeometryMeshContent> MeshList;

        public GeometryModelContent()
        {
            this.MeshList = new List<GeometryMeshContent>();
        }

        public IReadOnlyList<GeometryMeshContent> Meshes => this.MeshList;

        public void Add(GeometryMeshContent mesh)
            => this.MeshList.Add(mesh);
    }
}
