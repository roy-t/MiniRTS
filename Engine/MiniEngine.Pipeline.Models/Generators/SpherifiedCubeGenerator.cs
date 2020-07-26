using System.Collections.Generic;
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
    public class SpherifiedCubeGenerator
    {
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
            var indices = new List<short>();

            // Front
            GenerateFace(Vector3.Right, Vector3.Up, Vector3.Backward, radius, subdivisions, vertices, indices);

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

            return this.GeometryFactory.Construct(entity, vertices.ToArray(), indices.ToArray(), PrimitiveType.TriangleList);
        }


        private static void GenerateFace(Vector3 right, Vector3 up, Vector3 backward, float radius, int subdivisions, List<GBufferVertex> vertices, List<short> indices)
        {
            var start = vertices.Count;
            var currentIndex = (short)(indices.Union(new short[] { -1 }).Max() + 1);

            var faceCenter = backward * radius;
            var topLeft = faceCenter + (-right * radius) + (up * radius);

            var verticesPerEdge = subdivisions + 2;
            var indexLookup = new short[verticesPerEdge, verticesPerEdge];

            for (var column = 0; column < verticesPerEdge; column++)
            {
                for (var row = 0; row < verticesPerEdge; row++)
                {
                    var columnWidth = (radius * 2) / (verticesPerEdge - 1);
                    var columnHeight = (radius * 2) / (verticesPerEdge - 1);

                    var position = topLeft + (right * column * columnWidth) + (-up * row * columnHeight);
                    vertices.Add(new GBufferVertex(position));

                    indexLookup[column, row] = currentIndex++;

                    if (column > 0 && row > 0)
                    {
                        indices.Add(indexLookup[column - 1, row - 1]);
                        indices.Add(indexLookup[column, row - 1]);
                        indices.Add(indexLookup[column, row]);

                        indices.Add(indexLookup[column, row]);
                        indices.Add(indexLookup[column - 1, row]);
                        indices.Add(indexLookup[column - 1, row - 1]);
                    }
                }
            }

            var length = vertices.Count - start;
            SpherifyFace(vertices, start, length, up, radius);
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
