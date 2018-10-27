using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Primitives.Bounds
{
    public static class ModelExtensions
    {
        // From: https://gamedev.stackexchange.com/questions/2438/how-do-i-create-bounding-boxes-with-xna-4-0

        public static BoundingBox ComputeBoundingBox(this Model model, Matrix worldTransform)
        {
            ComputeExtremes(model, worldTransform, out var min, out var max);
            return new BoundingBox(min, max);
        }

        public static BoundingSphere ComputeBoundingSphere(this Model model, Matrix worldTransform)
        {
            ComputeExtremes(model, worldTransform, out var min, out var max);
            return BoundingSphere.CreateFromPoints(new[] { min, max });
        }

        private static void ComputeExtremes(Model model, Matrix worldTransform, out Vector3 min, out Vector3 max)
        {
            // Initialize minimum and maximum corners of the bounding box to max and min values
            min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var absoluteBoneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(absoluteBoneTransforms);

            // For each mesh of the model
            foreach (var mesh in model.Meshes)
            {
                var localWorldTransform = absoluteBoneTransforms[mesh.ParentBone.Index] * worldTransform;

                foreach (var meshPart in mesh.MeshParts)
                {
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
    }
}
