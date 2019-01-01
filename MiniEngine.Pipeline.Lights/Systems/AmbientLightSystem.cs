using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using System;
using System.Collections.Generic;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class AmbientLightSystem : ISystem
    {
        private const int KernelSize = 64;

        private readonly GraphicsDevice Device;
        private readonly AmbientLightEffect Effect;
        private readonly BlurEffect BlurEffect;
        private readonly EntityLinker EntityLinker;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly List<AmbientLight> Lights;
        private readonly Vector3[] Kernel;
        private readonly Texture2D NoiseMap;

        public AmbientLightSystem(GraphicsDevice device, AmbientLightEffect effect, BlurEffect blurEffect, EntityLinker entityLinker)
        {
            this.Device = device;
            this.Effect = effect;
            this.BlurEffect = blurEffect;
            this.EntityLinker = entityLinker;
            this.FullScreenTriangle = new FullScreenTriangle();
            this.Lights = new List<AmbientLight>();

            this.Kernel = this.GenerateKernel();

            this.NoiseMap = new Texture2D(device, 64, 64, false, SurfaceFormat.Color);
            var random = new Random(255);
            SimplexNoise.Seed = random.Next();
            var noiseX = SimplexNoise.Calc2D(this.NoiseMap.Width, this.NoiseMap.Height, 1.0f);
            SimplexNoise.Seed = random.Next();
            var noiseY = SimplexNoise.Calc2D(this.NoiseMap.Width, this.NoiseMap.Height, 1.0f);

            SimplexNoise.Seed = random.Next();
            var noiseZ = SimplexNoise.Calc2D(this.NoiseMap.Width, this.NoiseMap.Height, 1.0f);

            var noise = new Color[this.NoiseMap.Width * this.NoiseMap.Height];
            for (var y = 0; y < this.NoiseMap.Height; y++)
            {
                for (var x = 0; x < this.NoiseMap.Width; x++)
                {
                    var r = (noiseX[x, y] / 128.0f) - 1.0f;
                    var g = (noiseY[x, y] / 128.0f) - 1.0f;
                    var b = (noiseZ[x, y] / 128.0f) - 1.0f;

                    noise[x * y] = new Color(r, g, b);
                }
            }

            this.NoiseMap.SetData(noise);
        }

        public void Render(PerspectiveCamera camera, RenderTarget2D normalTarget, RenderTarget2D depthTarget)
        {
            var ambientLight = this.ComputeAmbientLightZeroAlpha();

            using (this.Device.ShadowCastingLightState())
            {
                // G-Buffer input
                this.Effect.NormalMap = normalTarget;
                this.Effect.DepthMap = depthTarget;
                this.Effect.FilteredDepthMap = depthTarget;
                this.Effect.NoiseMap = this.NoiseMap;

                // Light properties
                this.Effect.Color = ambientLight;
                this.Effect.Kernel = this.Kernel;

                // Camera properties
                this.Effect.View = camera.View;
                this.Effect.Projection = camera.Projection;
                this.Effect.InverseViewProjection = camera.InverseViewProjection;

                this.Effect.Apply();
                this.FullScreenTriangle.Render(this.Device);
            }
        }

        public void Blur(PerspectiveCamera camera, RenderTarget2D sourceTarget, RenderTarget2D depthTarget)
        {
            using (this.Device.ShadowCastingLightState())
            {
                // G-Buffer input
                this.BlurEffect.DepthMap = depthTarget;

                this.BlurEffect.SourceMap = sourceTarget;
                this.BlurEffect.MaxDistance = camera.FarPlane;

                this.BlurEffect.Apply();
                this.FullScreenTriangle.Render(this.Device);
            }
        }

        private Color ComputeAmbientLightZeroAlpha()
        {
            this.Lights.Clear();
            this.EntityLinker.GetComponents(this.Lights);

            var accumulate = Color.TransparentBlack;
            foreach (var light in this.Lights)
            {
                accumulate.R += light.Color.R;
                accumulate.G += light.Color.G;
                accumulate.B += light.Color.B;
            }

            return accumulate;
        }

        private Vector3[] GenerateKernel()
        {
            var random = new Random(369);

            var kernel = new Vector3[KernelSize];

            for (var i = 0; i < KernelSize; i++)
            {
                var scale = (float)i / (float)KernelSize;
                var v = new Vector3
                (
                    (2.0f * (float)random.NextDouble()) - 1.0f,
                    (2.0f * (float)random.NextDouble()) - 1.0f,
                    (2.0f * (float)random.NextDouble()) - 1.0f
                );

                v *= 0.1f + (0.9f * scale * scale);

                kernel[i] = v;
            }

            return kernel;
        }
    }
}