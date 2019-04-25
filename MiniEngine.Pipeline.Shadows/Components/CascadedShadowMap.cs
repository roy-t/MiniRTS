using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Systems.Annotations;
using MiniEngine.Systems.Components;

namespace MiniEngine.Pipeline.Shadows.Components
{
    [Label(nameof(CascadedShadowMap))]
    public sealed class CascadedShadowMap : IComponent
    {
        public CascadedShadowMap(GraphicsDevice device, int resolution, int cascades,
            Vector3 position, Vector3 lookAt, float[] cascadeDistances)
        {
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

            this.Move(position, lookAt);
        }

        [Editor(nameof(Cascades), nameof(Cascades))]
        public int Cascades => this.CascadeSplits.Length;

        public RenderTarget2D DepthMapArray { get; }
        public RenderTarget2D ColorMapArray { get; }

        public CascadeCamera[] ShadowCameras { get; }

        public float[] CascadeSplits { get; }
        public Vector4[] CascadeOffsets { get; }
        public Vector4[] CascadeScales { get; }
        public Matrix GlobalShadowMatrix { get; set; }

        [Editor(nameof(Position), nameof(Position))]
        public Vector3 Position { get; private set; }

        [Editor(nameof(LookAt), nameof(LookAt))]
        public Vector3 LookAt { get; private set; }

        public Vector3 SurfaceToLightVector { get; private set; }

        public int Resolution { get; }
        public float[] CascadeDistances { get; }

        public void Move(Vector3 position, Vector3 lookAt)
        {
            this.Position = position;
            this.LookAt = lookAt;
            this.SurfaceToLightVector = Vector3.Normalize(position - lookAt);
        }       
    }
}
