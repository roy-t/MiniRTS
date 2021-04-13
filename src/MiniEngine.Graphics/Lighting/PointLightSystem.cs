using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;
using MiniEngine.Graphics.Lighting.Mathematics;
using MiniEngine.Graphics.Lighting.Volumes;
using MiniEngine.Graphics.Physics;
using MiniEngine.Systems;
using MiniEngine.Systems.Generators;

namespace MiniEngine.Graphics.Lighting
{
    [System]
    public partial class PointLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly FrameService FrameService;
        private readonly PointLightEffect Effect;
        private readonly SphereLightVolume Volume;

        private readonly RasterizerState InsideRasterizer;
        private readonly RasterizerState OutsideRasterizer;

        public PointLightSystem(GraphicsDevice device, SphereLightVolume volume, PointLightEffect effect, FrameService frameService)
        {
            this.Device = device;
            this.FrameService = frameService;
            this.Volume = volume;
            this.Effect = effect;

            this.InsideRasterizer = new RasterizerState()
            {
                CullMode = CullMode.CullClockwiseFace,
                DepthClipEnable = false
            };

            this.OutsideRasterizer = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace,
                DepthClipEnable = false
            };
        }

        public void OnSet()
        {
            this.Device.BlendState = BlendState.Additive;
            this.Device.DepthStencilState = DepthStencilState.None;

            this.Device.SetRenderTarget(this.FrameService.LBuffer.Light);
        }

        [ProcessAll]
        public void Process(PointLightComponent pointLight, TransformComponent transform)
        {
            var camera = this.FrameService.CameraComponent.Camera;

            var radius = LightMath.LightVolumeRadius(pointLight.Strength);
            var world = Matrix.CreateScale(radius) * transform.Matrix;

            var inside = Vector3.Distance(camera.Position, transform.Matrix.Translation) < radius;
            this.Device.RasterizerState = inside
                ? this.InsideRasterizer
                : this.OutsideRasterizer;

            this.Effect.WorldViewProjection = world * this.FrameService.CameraComponent.Camera.ViewProjection;
            this.Effect.CameraPosition = this.FrameService.CameraComponent.Camera.Position;

            this.Effect.Albedo = this.FrameService.GBuffer.Albedo;
            this.Effect.Normal = this.FrameService.GBuffer.Normal;
            this.Effect.Depth = this.FrameService.GBuffer.Depth;
            this.Effect.Material = this.FrameService.GBuffer.Material;
            this.Effect.GBufferSampler = SamplerState.LinearClamp;


            this.Effect.InverseViewProjection = Matrix.Invert(camera.ViewProjection);

            this.Effect.Position = transform.Matrix.Translation;
            this.Effect.Color = pointLight.Color.ToVector4();
            this.Effect.Strength = pointLight.Strength;

            this.Effect.Apply();

            this.Volume.Render(this.Device);
        }
    }
}
