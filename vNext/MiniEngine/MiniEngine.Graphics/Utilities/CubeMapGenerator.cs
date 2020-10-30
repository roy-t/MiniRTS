using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;

namespace MiniEngine.Graphics.Utilities
{
    [System]
    public sealed class CubeMapGenerator
    {
        private readonly GraphicsDevice Device;
        private readonly CubeMapGeneratorEffect Effect;

        public CubeMapGenerator(GraphicsDevice device, EffectFactory effectFactory)
        {
            this.Device = device;
            this.Effect = effectFactory.Construct<CubeMapGeneratorEffect>();
        }

        public TextureCube Generate(Texture2D equirectangularTexture)
        {
            var resolution = equirectangularTexture.Height / 2;
            var cubeMap = new TextureCube(this.Device, resolution, false, SurfaceFormat.HalfVector4);

            using var faceRenderTarget = new RenderTarget2D(this.Device, resolution, resolution, false, SurfaceFormat.HalfVector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            using var cube = new Cube(this.Device);

            var data = new HalfVector4[resolution * resolution];
            var faces = (CubeMapFace[])Enum.GetValues(typeof(CubeMapFace));
            for (var i = 0; i < faces.Length; i++)
            {
                var face = faces[i];
                var view = GetViewForFace(face);
                var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 0.1f, 1.5f);

                this.Device.SetRenderTarget(faceRenderTarget);
                this.Device.Clear(Color.CornflowerBlue);

                this.Effect.EquirectangularTexture = equirectangularTexture;
                this.Effect.WorldViewProjection = view * projection;

                this.Effect.Apply();

                this.Device.SetVertexBuffer(cube.VertexBuffer, 0);
                this.Device.Indices = cube.IndexBuffer;
                this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, cube.Primitives);

                this.Device.SetRenderTarget(null);

                faceRenderTarget.GetData(data);
                cubeMap.SetData(face, data);
            }

            return cubeMap;
        }

        private static Matrix GetViewForFace(CubeMapFace face)
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
