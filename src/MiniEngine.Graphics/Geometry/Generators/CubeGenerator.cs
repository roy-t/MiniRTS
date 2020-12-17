using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.ContentPipeline.Shared;

namespace MiniEngine.Graphics.Geometry.Generators
{
    public static class CubeGenerator
    {
        private const float BoundingRadius = 1.41421356237f; // square root of 2

        public static GeometryData Generate(GraphicsDevice device)
        {
            var vertices = new List<GeometryVertex>(4 * 6);
            var indices = new List<short>(6 * 6);

            // Front
            GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Up, Vector3.Backward), vertices, indices);

            // Back
            GenerateFace(new CoordinateSystem(Vector3.Left, Vector3.Up, Vector3.Forward), vertices, indices);

            // Left
            GenerateFace(new CoordinateSystem(Vector3.Backward, Vector3.Up, Vector3.Left), vertices, indices);

            // Right
            GenerateFace(new CoordinateSystem(Vector3.Forward, Vector3.Up, Vector3.Right), vertices, indices);

            // Top
            GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Forward, Vector3.Up), vertices, indices);

            // Botom
            GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Backward, Vector3.Down), vertices, indices);

            var vertexBuffer = new VertexBuffer(device, GeometryVertex.Declaration, vertices.Count, BufferUsage.None);
            vertexBuffer.SetData(vertices.ToArray());

            var indexBuffer = new IndexBuffer(device, IndexElementSize.SixteenBits, indices.Count, BufferUsage.None);
            indexBuffer.SetData(indices.ToArray());
            return new GeometryData(vertexBuffer, indexBuffer, new BoundingSphere(Vector3.Zero, BoundingRadius));
        }

        private static void GenerateFace(CoordinateSystem coordinateSystem, List<GeometryVertex> vertices, List<short> indices)
        {
            var maxX = coordinateSystem.UnitX / 2.0f;
            var maxY = coordinateSystem.UnitY / 2.0f;
            var maxZ = coordinateSystem.UnitZ / 2.0f;
            var normal = Vector3.Normalize(maxZ);

            var topLeft = -maxX + maxY + maxZ;
            var topRight = maxX + maxY + maxZ;
            var bottomRight = maxX - maxY + maxZ;
            var bottomLeft = -maxX - maxY + maxZ;

            var topLeftIndex = (short)(vertices.Count + 0);
            var topRightIndex = (short)(vertices.Count + 1);
            var bottomRightIndex = (short)(vertices.Count + 2);
            var bottomLeftIndex = (short)(vertices.Count + 3);

            vertices.Add(new GeometryVertex(topLeft, new Vector2(0, 0), normal));
            vertices.Add(new GeometryVertex(topRight, new Vector2(1, 0), normal));
            vertices.Add(new GeometryVertex(bottomRight, new Vector2(1, 1), normal));
            vertices.Add(new GeometryVertex(bottomLeft, new Vector2(0, 1), normal));

            indices.Add(topLeftIndex);
            indices.Add(topRightIndex);
            indices.Add(bottomRightIndex);

            indices.Add(bottomRightIndex);
            indices.Add(bottomLeftIndex);
            indices.Add(topLeftIndex);
        }
    }
}
