using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MiniEngine.ContentPipeline.Shared
{
    public sealed class GeometryModel
    {
        private readonly List<GeometryMesh> MeshList;

        public GeometryModel()
        {
            this.MeshList = new List<GeometryMesh>(1);
            this.Bounds = new BoundingSphere(Vector3.Zero, 0.0f);
        }

        public GeometryModel(GeometryData geometry, Material material)
            : this()
        {
            var mesh = new GeometryMesh(geometry, material, Matrix.Identity);
            this.Add(mesh);
        }

        public IReadOnlyList<GeometryMesh> Meshes => this.MeshList;

        public BoundingSphere Bounds { get; private set; }

        public void Add(GeometryMesh mesh)
        {
            this.MeshList.Add(mesh);

            var bounds = mesh.Geometry.Bounds;
            bounds.Transform(mesh.Offset);

            if (this.Meshes.Count == 0)
            {
                this.Bounds = bounds;
            }
            else
            {
                this.Bounds = BoundingSphere.CreateMerged(this.Bounds, bounds);
            }
        }
    }
}
