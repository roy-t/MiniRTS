//using System.Collections.Generic;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;

//namespace MiniEngine.Graphics.Geometry.Generators
//{
//    public static class CubeGenerator
//    {
//        public static Geometry Generate(GraphicsDevice device)
//        {
//            var vertices = new List<GeometryVertex>(4 * 6);
//            var indices = new List<short>(6 * 6);

// // Front GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Up, Vector3.Backward),
// vertices, indices);

// // Back GenerateFace(new CoordinateSystem(Vector3.Left, Vector3.Up, Vector3.Forward), vertices, indices);

// // Left GenerateFace(new CoordinateSystem(Vector3.Backward, Vector3.Up, Vector3.Left), vertices, indices);

// // Right GenerateFace(new CoordinateSystem(Vector3.Forward, Vector3.Up, Vector3.Right), vertices, indices);

// // Top GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Forward, Vector3.Up), vertices, indices);

// // Botom GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Backward, Vector3.Down),
// vertices, indices);

// var vertexBuffer = new VertexBuffer(device, GeometryVertex.Declaration, vertices.Count,
// BufferUsage.None); vertexBuffer.SetData(vertices.ToArray());

// var indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Count,
// BufferUsage.None); indexBuffer.SetData(indices.ToArray()); return new Geometry(vertexBuffer,
// indexBuffer); }

// private static void GenerateFace(CoordinateSystem coordinateSystem, List<GeometryVertex>
// vertices, List<short> indices) { var maxX = coordinateSystem.UnitX; var maxY =
// coordinateSystem.UnitY; var maxZ = coordinateSystem.UnitZ;

// var topLeft = -maxX + maxY + maxZ; var topRight = maxX + maxY + maxZ; var bottomRight = maxX -
// maxY + maxZ; var bottomLeft = -maxX - maxY + maxZ;

// var topLeftIndex = (short)(vertices.Count + 0); var topRightIndex = (short)(vertices.Count + 1);
// var bottomRightIndex = (short)(vertices.Count + 2); var bottomLeftIndex = (short)(vertices.Count
// + 3);

//            vertices.Add(new GeometryVertex(topLeft, new Vector2(0, 0), maxZ, maxX, maxY))
//        }
//    }
//}
