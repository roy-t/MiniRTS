using System;
using Microsoft.Xna.Framework;
using MiniEngine.Primitives.VertexTypes;

namespace MiniEngine.Pipeline.Models.Generators
{
    public static class MeshHelper
    {
        public static void ComputeNormals(GBufferVertex[] vertices, int[] indices)
        {
            var normals = new Vector3[vertices.Length];
            for (var i = 0; i < indices.Length; i += 3)
            {
                var i0 = indices[i + 0];
                var i1 = indices[i + 1];
                var i2 = indices[i + 2];

                var v0 = vertices[i0];
                var v1 = vertices[i1];
                var v2 = vertices[i2];

                var normal = Vector3.Cross(v2.PositionXYZ - v1.PositionXYZ, v1.PositionXYZ - v0.PositionXYZ);
                var length = normal.Length();
                if (length > 0.0f)
                {
                    normal = normal / length;
                    normals[i0] += normal;
                    normals[i1] += normal;
                    normals[i2] += normal;
                }
            }

            for (var i = 0; i < normals.Length; i++)
            {
                var normal = Vector3.Normalize(normals[i]);
                vertices[i].Normal = normal;
            }
        }

        public static void ScaleToUnitSphere(GBufferVertex[] vertices)
        {
            var max = 0.0f;
            for (var i = 0; i < vertices.Length; i++)
            {
                var distance = vertices[i].Position.Length();
                max = Math.Max(max, distance);
            }

            for (var i = 0; i < vertices.Length; i++)
            {
                vertices[i].Position = vertices[i].Position / max;
            }
        }
    }
}