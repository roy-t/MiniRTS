using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Models.Components;

namespace MiniEngine.Pipeline.Models.Factories
{
    public static class BoundaryComputer
    {
        public static BoundingSphere FromMinMax(Vector3 min, Vector3 max)
            => new BoundingSphere(Vector3.Lerp(min, max, 0.5f), Vector3.Distance(min, max) * 0.5f);

        public static void ComputeExtremes(Model model, out Vector3 min, out Vector3 max)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var absoluteBoneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(absoluteBoneTransforms);

            // For each mesh of the model
            for (var iMesh = 0; iMesh < model.Meshes.Count; iMesh++)
            {
                var mesh = model.Meshes[iMesh];
                var localWorldTransform = absoluteBoneTransforms[mesh.ParentBone.Index];

                for (var iPart = 0; iPart < mesh.MeshParts.Count; iPart++)
                {
                    var meshPart = mesh.MeshParts[iPart];
                    // Vertex buffer parameters
                    var vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    var vertexBufferSize = meshPart.NumVertices * vertexStride;

                    // Get vertex data as float
                    var vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    // Iterate through vertices (possibly) growing bounding box, all calculations are done in world space
                    for (var i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        var transformedPosition = Vector3.Transform(new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]), localWorldTransform);

                        min = Vector3.Min(min, transformedPosition);
                        max = Vector3.Max(max, transformedPosition);
                    }
                }
            }
        }

        public static void ComputeExtremes(Geometry geometry, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            for (var i = 0; i < geometry.VertexCount; i++)
            {
                var vertex = geometry.Vertices[i];
                var position = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);
                min = Vector3.Min(min, position);
                max = Vector3.Max(max, position);
            }
        }
    }
}
