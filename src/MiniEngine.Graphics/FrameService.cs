using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Camera;
using MiniEngine.Graphics.Geometry;
using MiniEngine.Graphics.Lighting;
using MiniEngine.Graphics.PostProcess;
using MiniEngine.Graphics.Shadows;
using MiniEngine.Graphics.Skybox;
using MiniEngine.Systems;
using MiniEngine.Systems.Components;
using MiniEngine.Systems.Entities;

namespace MiniEngine.Graphics
{
    [Service]
    public sealed class FrameService
    {
        private static readonly float[] DefaultCascadeDistances =
       {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };
        private readonly EntityAdministrator Entities;
        private readonly ComponentAdministrator Components;
        private readonly GraphicsDevice Device;

        public FrameService(EntityAdministrator entities, ComponentAdministrator components, GraphicsDevice device)
        {
            this.Entities = entities;
            this.Components = components;
            this.Device = device;
            this.Skybox = null!;
            this.GBuffer = new GBuffer(device);
            this.LBuffer = new LBuffer(device);
            this.PBuffer = new PBuffer(device);

            this.Reset();
        }

        public float Elapsed { get; set; }

        public Entity PrimaryCameraEntity { get; set; }

        public CameraComponent CameraComponent => this.Components.GetComponent<CameraComponent>(this.PrimaryCameraEntity);

        public Entity PrimaryLightSourceEntity { get; set; }

        public SunlightComponent Sunlight => this.Components.GetComponent<SunlightComponent>(this.PrimaryLightSourceEntity);

        public CascadedShadowMapComponent ShadowMap => this.Components.GetComponent<CascadedShadowMapComponent>(this.PrimaryLightSourceEntity);

        public GBuffer GBuffer { get; set; }
        public LBuffer LBuffer { get; set; }
        public PBuffer PBuffer { get; set; }

        public SkyboxGeometry Skybox { get; set; } // TODO: this field should be replaced by a service that searches for the best skybox given the objects position

        public void Reset()
        {
            this.PrimaryCameraEntity = this.CreatePrimaryCamera();
            this.PrimaryLightSourceEntity = this.CreatePrimaryLightSource();
        }

        private Entity CreatePrimaryCamera()
        {
            var entity = this.Entities.Create();
            var cameraComponent = new CameraComponent(entity, new PerspectiveCamera(this.Device.Viewport.AspectRatio));
            this.Components.Add(cameraComponent);

            return entity;
        }

        private Entity CreatePrimaryLightSource()
        {
            var entity = this.Entities.Create();
            this.Components.Add(new SunlightComponent(entity, Color.White, 3));
            this.Components.Add(CascadedShadowMapComponent.Create(entity, this.Device, 2048, DefaultCascadeDistances));

            var position = Vector3.Up;
            var lookAt = (Vector3.Left * 0.75f) + (Vector3.Backward * 0.1f);
            var forward = Vector3.Normalize(lookAt - position);

            var camera = new PerspectiveCamera(1.0f, position, forward);
            this.Components.Add(new CameraComponent(entity, camera));

            return entity;
        }


        public int GetBufferSize()
        {
            var gBufferSize = BufferSize(this.GBuffer.Depth, this.GBuffer.Albedo, this.GBuffer.Material, this.GBuffer.Normal);
            var lBufferSize = BufferSize(this.LBuffer.Light);
            var skyboxSize = BufferSize(this.Skybox.Environment, this.Skybox.Irradiance, this.Skybox.Texture);

            return gBufferSize + lBufferSize + skyboxSize;
        }

        private static int BufferSize(params TextureCube[] textures)
            => textures.Sum(x => TextureSize(x));

        private static int TextureSize(TextureCube texture)
        {
            var sum = 0;
            var size = texture.Size;
            for (var l = 0; l < texture.LevelCount; l++)
            {
                sum += size * size * FormatSizeInBytes(texture.Format) * 6;
                size /= 2;
            }

            return sum;
        }

        private static int BufferSize(params Texture2D[] textures)
            => textures.Sum(x => TextureSize(x));

        private static int TextureSize(Texture2D texture)
            => texture.Width * texture.Height * FormatSizeInBytes(texture.Format);

        private static int FormatSizeInBytes(SurfaceFormat format)
            => format switch
            {
                SurfaceFormat.Color => 4,
                SurfaceFormat.HalfSingle => 2,
                SurfaceFormat.Single => 4,
                SurfaceFormat.Vector2 => 8,
                SurfaceFormat.Vector4 => 32,
                SurfaceFormat.HalfVector2 => 4,
                SurfaceFormat.HalfVector4 => 8,
                SurfaceFormat.ColorSRgb => 4,
                _ => throw new ArgumentOutOfRangeException(nameof(format)),
            };
    }
}
