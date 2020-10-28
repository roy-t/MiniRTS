using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Effects;
using MiniEngine.Graphics.Skybox;
using Serilog;

namespace MiniEngine.Graphics.Utilities
{
    [System]
    public sealed class CubeMapGenerator
    {
        private readonly GraphicsDevice Device;
        private readonly CubeMapGeneratorEffect Effect;
        private readonly ILogger Logger;

        public CubeMapGenerator(ILogger logger, GraphicsDevice device, EffectFactory effectFactory)
        {
            this.Logger = logger;
            this.Device = device;
            this.Effect = effectFactory.Construct<CubeMapGeneratorEffect>();
        }

        public TextureCube Generate(Texture2D equirectangularTexture, int resolution)
        {
            var geometry = SkyboxGenerator.Generate(this.Device, equirectangularTexture);

            var faces = (CubeMapFace[])Enum.GetValues(typeof(CubeMapFace));

            var faceRenderTargets = new RenderTarget2D[faces.Length];

            var cube = new TextureCube(this.Device, resolution, false, equirectangularTexture.Format);

            for (var i = 0; i < faces.Length; i++)
            {
                var face = faces[i];
                faceRenderTargets[i] = new RenderTarget2D(this.Device, resolution, resolution, false, equirectangularTexture.Format, DepthFormat.None, 0, RenderTargetUsage.PreserveContents); // TODO: discard contents?
                var view = GetViewForFace(face);
                var projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 0.1f, 1.5f);

                this.Device.SetRenderTarget(faceRenderTargets[i]);
                this.Device.Clear(Color.CornflowerBlue);

                this.Effect.EquirectangularTexture = equirectangularTexture;
                this.Effect.WorldViewProjection = view * projection;

                this.Effect.Apply();

                this.Device.SetVertexBuffer(geometry.VertexBuffer, 0);
                this.Device.Indices = geometry.IndexBuffer;
                this.Device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, geometry.Primitives);

                this.Device.SetRenderTarget(null);

                var file = $"Face_{face}_{i}.png";
                var path = Path.GetFullPath(file);
                this.Logger.Information($"Writing cube to {path}");
                var stream = File.Create(path);
                faceRenderTargets[i].SaveAsPng(stream, resolution, resolution);
                stream.Flush();
                stream.Dispose();
            }

            return cube;
        }

        private static Matrix GetViewForFace(CubeMapFace face)
        => face switch
        {
            CubeMapFace.PositiveX => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitX, Vector3.Up),
            CubeMapFace.NegativeX => Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitX, Vector3.Up),
            CubeMapFace.PositiveY => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitY, Vector3.Backward),
            CubeMapFace.NegativeY => Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitY, Vector3.Forward),
            CubeMapFace.PositiveZ => Matrix.CreateLookAt(Vector3.Zero, Vector3.UnitZ, Vector3.Up),
            CubeMapFace.NegativeZ => Matrix.CreateLookAt(Vector3.Zero, -Vector3.UnitZ, Vector3.Up),
            _ => throw new ArgumentOutOfRangeException(nameof(face))
        };
    }
}
