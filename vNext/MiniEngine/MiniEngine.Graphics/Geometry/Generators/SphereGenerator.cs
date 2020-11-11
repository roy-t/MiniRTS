using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MiniEngine.Graphics.Geometry.Generators
{
    // Uses the spherified cube algorithm to generate a sphere Inspiration:
    // - https://medium.com/game-dev-daily/four-ways-to-create-a-mesh-for-a-sphere-d7956b825db4
    // - https://scaryreasoner.wordpress.com/2016/01/23/thoughts-on-tesselating-a-sphere/
    public static class SphereGenerator
    {
        public static Geometry Generate(GraphicsDevice device, int subdivisions)
        {
            var vertices = new List<GeometryVertex>();
            var indices = new List<int>();

            // Front
            GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Up, Vector3.Backward), subdivisions, vertices, indices);

            // Back
            GenerateFace(new CoordinateSystem(Vector3.Left, Vector3.Up, Vector3.Forward), subdivisions, vertices, indices);

            // Left
            GenerateFace(new CoordinateSystem(Vector3.Backward, Vector3.Up, Vector3.Left), subdivisions, vertices, indices);

            // Right
            GenerateFace(new CoordinateSystem(Vector3.Forward, Vector3.Up, Vector3.Right), subdivisions, vertices, indices);

            // Top
            GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Forward, Vector3.Up), subdivisions, vertices, indices);

            // Botom
            GenerateFace(new CoordinateSystem(Vector3.Right, Vector3.Backward, Vector3.Down), subdivisions, vertices, indices);

            var vertexBuffer = new VertexBuffer(device, GeometryVertex.Declaration, vertices.Count, BufferUsage.None);
            vertexBuffer.SetData(vertices.ToArray());

            var indexBuffer = new IndexBuffer(device, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.None);
            indexBuffer.SetData(indices.ToArray());

            return new Geometry(vertexBuffer, indexBuffer);
        }

        private static void GenerateFace(CoordinateSystem coordinateSystem, int subdivisions, List<GeometryVertex> vertices, List<int> indices)
        {
            var quads = new List<Quad>();
            var currentIndex = indices.Append(-1).Max() + 1;

            var maxX = coordinateSystem.UnitX;
            var maxY = coordinateSystem.UnitY;
            var maxZ = coordinateSystem.UnitZ;

            var topLeft = -maxX + maxY + maxZ;
            var topRight = maxX + maxY + maxZ;
            var bottomRight = maxX - maxY + maxZ;
            var bottomLeft = -maxX - maxY + maxZ;

            var verticesPerEdge = subdivisions + 2;
            var indexLookup = new int[verticesPerEdge, verticesPerEdge];

            for (var column = 0; column < verticesPerEdge; column++)
            {
                for (var row = 0; row < verticesPerEdge; row++)
                {
                    var x = FaceLerp(column / (verticesPerEdge - 1.0f));
                    var y = FaceLerp(row / (verticesPerEdge - 1.0f));

                    var centerLeft = Vector3.Lerp(topLeft, bottomLeft, y);
                    var r = Vector3.Lerp(topRight, bottomRight, y);

                    var position = Vector3.Normalize(Vector3.Lerp(centerLeft, r, x));
                    var texture = new Vector2(x, y);
                    vertices.Add(new GeometryVertex(position, texture, position));

                    indexLookup[column, row] = currentIndex++;

                    if (column > 0 && row > 0)
                    {
                        var topLeftIndex = indexLookup[column - 1, row - 1];
                        var topRightIndex = indexLookup[column, row - 1];
                        var bottomRightIndex = indexLookup[column, row];
                        var bottomLeftIndex = indexLookup[column - 1, row];

                        quads.Add(new Quad(topLeftIndex, topRightIndex, bottomRightIndex, bottomLeftIndex));
                    }
                }
            }

            TriangulateFace(vertices, quads, indices);
        }

        private static float FaceLerp(float amount)
        {
            var angle = -MathHelper.PiOver4 + (amount * MathHelper.PiOver2);

            var basis = Vector3.UnitZ;
            var v0 = Vector3.Transform(basis, Matrix.CreateRotationY(-MathHelper.PiOver4));
            var v1 = Vector3.Transform(basis, Matrix.CreateRotationY(MathHelper.PiOver4));

            var target = Vector3.Normalize(Vector3.Transform(basis, Matrix.CreateRotationY(angle)));

            var plane = new Plane(v0, v1, v1 + Vector3.UnitY);
            var ray = new Ray(Vector3.Zero, target);
            var distance = ray.Intersects(plane).GetValueOrDefault();

            var position = ray.Position + (ray.Direction * distance);

            var fullLength = Vector3.Distance(v0, v1);
            var partialLength = Vector3.Distance(v0, position);

            return partialLength / fullLength;
        }

        private static void TriangulateFace(List<GeometryVertex> vertices, List<Quad> quads, List<int> indices)
        {
            for (var i = 0; i < quads.Count; i++)
            {
                var quad = quads[i];
                var topLeftBottomRightDistance = Vector3.DistanceSquared(vertices[quad.TopLeftIndex].Position,
                    vertices[quad.BottomRightIndex].Position);

                var topRightBottomLeftDistance = Vector3.DistanceSquared(vertices[quad.TopRightIndex].Position,
                    vertices[quad.BottomLeftIndex].Position);

                if (topLeftBottomRightDistance < topRightBottomLeftDistance)
                {
                    indices.Add(quad.TopLeftIndex);
                    indices.Add(quad.TopRightIndex);
                    indices.Add(quad.BottomRightIndex);

                    indices.Add(quad.BottomRightIndex);
                    indices.Add(quad.BottomLeftIndex);
                    indices.Add(quad.TopLeftIndex);
                }
                else
                {
                    indices.Add(quad.TopRightIndex);
                    indices.Add(quad.BottomRightIndex);
                    indices.Add(quad.BottomLeftIndex);

                    indices.Add(quad.BottomLeftIndex);
                    indices.Add(quad.TopLeftIndex);
                    indices.Add(quad.TopRightIndex);
                }
            }
        }
    }
}
