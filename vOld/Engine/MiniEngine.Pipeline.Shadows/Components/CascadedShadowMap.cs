using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Shadows.Components
{
    public sealed class CascadedShadowMap : IComponent, IDisposable
    {
        public CascadedShadowMap(Entity entity, GraphicsDevice device, int resolution, int cascades,
            Vector3 position, Vector3 lookAt, float[] cascadeDistances)
        {
            this.Entity = entity;

            this.DepthMapArray = new RenderTarget2D(
                device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Single,
                DepthFormat.Depth24,
                0,
                RenderTargetUsage.DiscardContents,
                false,
                cascades);

            this.ColorMapArray = new RenderTarget2D(
                device,
                resolution,
                resolution,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.DiscardContents,
                false,
                cascades);

            this.ShadowCameras = new CascadeCamera[cascades];
            for (var i = 0; i < cascades; i++)
            {
                this.ShadowCameras[i] = new CascadeCamera();
            }

            this.CascadeSplits = new float[cascades];
            this.CascadeOffsets = new Vector4[cascades];
            this.CascadeScales = new Vector4[cascades];
            this.GlobalShadowMatrix = Matrix.Identity;
            this.Resolution = resolution;
            this.CascadeDistances = cascadeDistances;

            this.ShadowMaps = new ShadowMap[cascades];
            for (var i = 0; i < cascades; i++)
            {
                this.ShadowMaps[i] = new ShadowMap(entity, this.DepthMapArray, this.ColorMapArray, i, this.ShadowCameras[i]);
            }

            this.Move(position, lookAt);
        }

        public Entity Entity { get; }

        [Editor(nameof(Cascades))]
        public int Cascades => this.CascadeSplits.Length;

        public ShadowMap[] ShadowMaps { get; }

        [Editor(nameof(DepthMapArray))]
        public RenderTarget2D DepthMapArray { get; }

        [Editor(nameof(ColorMapArray))]
        public RenderTarget2D ColorMapArray { get; }

        public CascadeCamera[] ShadowCameras { get; }

        public float[] CascadeSplits { get; }
        public Vector4[] CascadeOffsets { get; }
        public Vector4[] CascadeScales { get; }
        public Matrix GlobalShadowMatrix { get; set; }

        [Editor(nameof(Position))]
        public Vector3 Position { get; private set; }

        [Editor(nameof(LookAt))]
        public Vector3 LookAt { get; private set; }

        public Vector3 SurfaceToLightVector { get; private set; }

        [Editor(nameof(Resolution))]
        public int Resolution { get; }

        [Editor(nameof(CascadeDistances), nameof(CascadeDistances), 0.0f, float.MaxValue)]
        public float[] CascadeDistances { get; set; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;
            this.SurfaceToLightVector = Vector3.Normalize(position - lookAt);
        }

        public void Dispose()
        {
            this.DepthMapArray?.Dispose();
            this.ColorMapArray?.Dispose();
        }
    }
}
