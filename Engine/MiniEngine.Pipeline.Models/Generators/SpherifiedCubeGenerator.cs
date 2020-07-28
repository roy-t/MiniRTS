using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Pipeline.Basics.Factories;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Primitives.VertexTypes;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Models.Generators
{
    // https://scaryreasoner.wordpress.com/2016/01/23/thoughts-on-tesselating-a-sphere/

    // Without equal angles method
    // Average area 0.0017018927
    // Max area 0.0031897912
    // Min area 0.00074976607
    // Std Dev 0.0006135705

    public class SpherifiedCubeGenerator
    {
        private static List<int[]> Triangles;

        private readonly GeometryFactory GeometryFactory;
        private readonly PoseFactory PoseFactory;
        private readonly EntityController EntityController;

        public SpherifiedCubeGenerator(GeometryFactory geometryFactory, PoseFactory poseFactory, EntityController entityController)
        {
            this.GeometryFactory = geometryFactory;
            this.PoseFactory = poseFactory;
            this.EntityController = entityController;
        }

        public Geometry Generate(float radius, int subdivisions)
        {
            var entity = this.EntityController.CreateEntity();
            this.PoseFactory.Construct(entity, Vector3.Zero);

            var vertices = new List<GBufferVertex>();
            var indices = new List<int>();

            Triangles = new List<int[]>();

            // Front
            GenerateFace(Vector3.Right, Vector3.Up, Vector3.Backward, radius, subdivisions, vertices, indices);

            if (true)
            {
                // Back
                GenerateFace(Vector3.Left, Vector3.Up, Vector3.Forward, radius, subdivisions, vertices, indices);

                // Left
                GenerateFace(Vector3.Backward, Vector3.Up, Vector3.Left, radius, subdivisions, vertices, indices);

                // Right
                GenerateFace(Vector3.Forward, Vector3.Up, Vector3.Right, radius, subdivisions, vertices, indices);

                // Top
                GenerateFace(Vector3.Right, Vector3.Forward, Vector3.Up, radius, subdivisions, vertices, indices);

                // Botom
                GenerateFace(Vector3.Right, Vector3.Backward, Vector3.Down, radius, subdivisions, vertices, indices);
            }

            CalculateStatistics(vertices);

            return this.GeometryFactory.Construct(entity, vertices.ToArray(), indices.ToArray(), PrimitiveType.TriangleList);
        }

        private static void CalculateStatistics(List<GBufferVertex> vertices)
        {
            var areas = new List<float>();

            foreach (var triangle in Triangles)
            {
                var width = Vector4.Distance(vertices[triangle[0]].Position, vertices[triangle[1]].Position);
                var height = Vector4.Distance(vertices[triangle[1]].Position, vertices[triangle[2]].Position);

                areas.Add(width * height * 0.5f);
            }

            var average = areas.Average();
            var stdDev = StdDev(areas);
            Debug.WriteLine($"Average area {average}M^2");
            Debug.WriteLine($"Min area {areas.Min()}m^2");
            Debug.WriteLine($"Max area {areas.Max()}m^2");
            Debug.WriteLine($"Std Dev {stdDev / average * 100.0f}%, {stdDev}m^2");
        }

        public static float StdDev(IEnumerable<float> values)
        {
            // ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/
            var mean = 0.0f;
            var sum = 0.0f;
            var stdDev = 0.0f;
            var n = 0;
            foreach (var val in values)
            {
                n++;
                var delta = val - mean;
                mean += delta / n;
                sum += delta * (val - mean);
            }

            if (1 < n)
            {
                stdDev = (float)Math.Sqrt(sum / n);
            }

            return stdDev;
        }

        private static void GenerateFace(Vector3 right, Vector3 up, Vector3 backward, float radius, int subdivisions, List<GBufferVertex> vertices, List<int> indices)
        {
            var start = vertices.Count;
            var currentIndex = indices.Union(new int[] { -1 }).Max() + 1;

            var faceCenter = backward * radius;
            var topLeft = faceCenter + (-right * radius) + (up * radius);
            var topRight = faceCenter + (right * radius) + (up * radius);

            var bottomRight = faceCenter + (right * radius) + (-up * radius);

            var bottomLeft = faceCenter + (-right * radius) + (-up * radius);

            var verticesPerEdge = subdivisions + 2;
            var indexLookup = new int[verticesPerEdge, verticesPerEdge];

            for (var column = 0; column < verticesPerEdge; column++)
            {
                for (var row = 0; row < verticesPerEdge; row++)
                {
                    var x = Amount(column, verticesPerEdge);
                    var y = Amount(row, verticesPerEdge);

                    var l = Vector3.Lerp(topLeft, bottomLeft, y);
                    var r = Vector3.Lerp(topRight, bottomRight, y);

                    var position = Vector3.Lerp(l, r, x);
                    vertices.Add(new GBufferVertex(position));

                    indexLookup[column, row] = currentIndex++;

                    if (column > 0 && row > 0)
                    {
                        var topLeftIndex = indexLookup[column - 1, row - 1];
                        var topRightIndex = indexLookup[column, row - 1];
                        var bottomRightIndex = indexLookup[column, row];
                        var bottomLeftIndex = indexLookup[column - 1, row];

                        var topLeftBottomRightDistance = Vector4.Distance(vertices[topLeftIndex].Position,
                            vertices[bottomRightIndex].Position);

                        var topRightBottomLeftDistance = Vector4.Distance(vertices[topRightIndex].Position,
                            vertices[bottomLeftIndex].Position);

                        if (topLeftBottomRightDistance < topRightBottomLeftDistance)
                        {
                            indices.Add(topLeftIndex);
                            indices.Add(topRightIndex);
                            indices.Add(bottomRightIndex);

                            indices.Add(bottomRightIndex);
                            indices.Add(bottomLeftIndex);
                            indices.Add(topLeftIndex);

                            Triangles.Add(new int[] { topLeftIndex, topRightIndex, bottomRightIndex });
                            Triangles.Add(new int[] { bottomRightIndex, bottomLeftIndex, topLeftIndex });
                        }
                        else
                        {
                            indices.Add(topRightIndex);
                            indices.Add(bottomRightIndex);
                            indices.Add(bottomLeftIndex);

                            indices.Add(bottomLeftIndex);
                            indices.Add(topLeftIndex);
                            indices.Add(topRightIndex);

                            Triangles.Add(new int[] { topRightIndex, bottomRightIndex, bottomLeftIndex });
                            Triangles.Add(new int[] { bottomLeftIndex, topLeftIndex, topRightIndex });
                        }
                    }
                }
            }

            var length = vertices.Count - start;
            SpherifyFace(vertices, start, length, up, radius);
        }

        private static float Amount(int i, int verticesPerEdge)
        {
            var x = i / (verticesPerEdge - 1.0f);
            var angle = -MathHelper.PiOver4 + (x * MathHelper.PiOver2);

            var basis = Vector3.UnitZ;
            var v0 = Vector3.Transform(basis, Matrix.CreateRotationY(-MathHelper.PiOver4));
            var v1 = Vector3.Transform(basis, Matrix.CreateRotationY(MathHelper.PiOver4));

            var target = Vector3.Normalize(Vector3.Transform(basis, Matrix.CreateRotationY(angle)));

            var plane = new Plane(v0, v1, v1 + Vector3.UnitY);
            var ray = new Ray(Vector3.Zero, target);
            var distance = ray.Intersects(plane).Value;

            var position = ray.Position + (ray.Direction * distance);

            var fullLength = Vector3.Distance(v0, v1);
            var partialLength = Vector3.Distance(v0, position);

            return partialLength / fullLength;
        }

        private static void SpherifyFace(List<GBufferVertex> vertices, int startIndex, int length, Vector3 pole, float radius)
        {
            for (var i = startIndex; i < startIndex + length; i++)
            {
                var vertex = vertices[i];

                var position = new Vector3(vertex.Position.X, vertex.Position.Y, vertex.Position.Z);

                var normal = Vector3.Normalize(position);
                var newPosition = normal * radius;

                var tangent = Vector3.Normalize(Vector3.Cross(pole, normal));
                var biNormal = Vector3.Normalize(Vector3.Cross(normal, tangent));

                vertices[i] = new GBufferVertex(newPosition, normal, tangent, biNormal);
            }
        }
    }
}

