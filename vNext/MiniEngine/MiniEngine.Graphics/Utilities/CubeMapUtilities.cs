using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    public static class CubeMapUtilities
    {
        public static IReadOnlyList<CubeMapFace> CubeMapFaces => (CubeMapFace[])Enum.GetValues(typeof(CubeMapFace));

        public static TextureCube RenderFaces(GraphicsDevice device, I3DEffect effect, int resolution, SurfaceFormat format)
        {
            var cubeMap = new TextureCube(device, resolution, false, format);

            using var faceRenderTarget = new RenderTarget2D(device, resolution, resolution, false, format, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            using var cube = new CubeMapCube(device);

            var data = new HalfVector4[resolution * resolution];
            var faces = CubeMapFaces;
            for (var i = 0; i < faces.Count; i++)
            {
                var face = faces[i];
                var view = GetViewForFace(face);
                var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 0.1f, 1.5f);

                device.SetRenderTarget(faceRenderTarget);
                device.Clear(Color.CornflowerBlue);

                effect.WorldViewProjection = view * projection;
                effect.Apply();

                device.SetVertexBuffer(cube.VertexBuffer, 0);
                device.Indices = cube.IndexBuffer;
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cube.Primitives);

                device.SetRenderTarget(null);

                faceRenderTarget.GetData(data);
                cubeMap.SetData(face, data);
            }

            return cubeMap;
        }

        public static Matrix GetViewForFace(CubeMapFace face)
        => face switch
        {
            CubeMapFace.PositiveX => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitX, Vector3.Up),
            CubeMapFace.NegativeX => Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitX, Vector3.Up),
            CubeMapFace.PositiveY => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitY, Vector3.Backward),
            CubeMapFace.NegativeY => Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitY, Vector3.Forward),
            // Invert Z as we assume a left handed (DirectX 9) coordinate system in the source texture
            CubeMapFace.PositiveZ => Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.Up),
            CubeMapFace.NegativeZ => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up),
            _ => throw new ArgumentOutOfRangeException(nameof(face))
        };
    }
}
