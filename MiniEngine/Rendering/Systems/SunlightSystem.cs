using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Effects;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;
using MiniEngine.Units;
using MiniEngine.Utilities.Extensions;

namespace MiniEngine.Rendering.Systems
{
    public sealed class SunlightSystem : IUpdatableSystem
    {
        public const int Resolution = 2048;
        public const int Cascades = 4;

        private static readonly float[] CascadeDistances =
        {
            0.075f,
            0.15f,
            0.3f,
            1.0f
        };

        private readonly GraphicsDevice Device;
        private readonly Frustum Frustum;
        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly ShadowMapSystem ShadowMapSystem;
        private readonly SunlightEffect Effect;

        private readonly Dictionary<Entity, Sunlight> Sunlights;

        public SunlightSystem(GraphicsDevice device, SunlightEffect effect, ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;
            this.Effect = effect;
            this.ShadowMapSystem = shadowMapSystem;

            this.FullScreenTriangle = new FullScreenTriangle();
            this.Frustum = new Frustum();

            this.Sunlights = new Dictionary<Entity, Sunlight>(1);
        }

        public bool Contains(Entity entity)
        {
            return this.Sunlights.ContainsKey(entity);
        }

        public string Describe(Entity entity)
        {
            var light = this.Sunlights[entity];
            return $"sun light, direction: {light.LookAt - light.LookAt}, color {light.Color}";
        }

        public void Remove(Entity entity)
        {
            this.Sunlights.Remove(entity);
            this.ShadowMapSystem.Remove(entity);
        }

        public void Update(PerspectiveCamera perspectiveCamera, Seconds elapsed)
        {
            foreach (var sunLight in this.Sunlights.Values)
            {
                ComputeCascades(sunLight, perspectiveCamera);
            }
        }

        public void Add(Entity entity, Color color, Vector3 position, Vector3 lookAt)
        {
            var sunlight = new Sunlight(color, position, lookAt, Cascades);

            this.ShadowMapSystem.Add(entity, sunlight.ShadowCameras, Cascades, Resolution);
            this.Sunlights.Add(entity, sunlight);
        }

        public void RemoveAll()
        {
            var keys = new Entity[this.Sunlights.Keys.Count];
            this.Sunlights.Keys.CopyTo(keys, 0);

            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public void RenderLights(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.ShadowCastingLightState())
            {
                foreach (var pair in this.Sunlights)
                {
                    var light = pair.Value;
                    var maps = this.ShadowMapSystem.Get(pair.Key);

                    RenderLight(light, maps.DepthMap, maps.ColorMap, perspectiveCamera, gBuffer);
                }
            }
        }

        private void ComputeCascades(Sunlight sunLight, PerspectiveCamera perspectiveCamera)
        {
            this.Frustum.ResetToViewVolume();
            this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
            sunLight.GlobalShadowMatrix =
                ShadowMath.CreateGlobalShadowMatrix(sunLight.SurfaceToLightVector, this.Frustum);

            for (var cascadeIndex = 0; cascadeIndex < Cascades; cascadeIndex++)
            {
                this.Frustum.ResetToViewVolume();
                // Transform to world space
                this.Frustum.Transform(perspectiveCamera.InverseViewProjection);

                // Slice the frustum
                var nearZ = cascadeIndex == 0 ? 0.0f : CascadeDistances[cascadeIndex - 1];
                var farZ = CascadeDistances[cascadeIndex];
                this.Frustum.Slice(nearZ, farZ);

                // Compute the shadow camera, a camera that sits on the surface of the bounding sphere
                // looking in the direction of the light
                var shadowCamera = ShadowMath.CreateShadowCamera(
                    sunLight.SurfaceToLightVector,
                    this.Frustum,
                    Resolution);
                sunLight.ShadowCameras[cascadeIndex] = shadowCamera;

                // ViewProjection matrix of the shadow camera that transforms to texture space [0, 1] instead of [-1, 1]
                var shadowMatrix = (shadowCamera.View * shadowCamera.Projection).TextureScaleTransform();

                // Store the split distance in terms of view space depth
                var clipDistance = perspectiveCamera.FarPlane - perspectiveCamera.NearPlane;
                sunLight.CascadeSplits[cascadeIndex] = perspectiveCamera.NearPlane + farZ * clipDistance;

                // Find scale and offset of this cascade in world space                    
                var invCascadeMat = Matrix.Invert(shadowMatrix);
                var cascadeCorner = Vector4.Transform(Vector3.Zero, invCascadeMat).ScaleToVector3();
                cascadeCorner = Vector4.Transform(cascadeCorner, sunLight.GlobalShadowMatrix).ScaleToVector3();

                // Do the same for the upper corner
                var otherCorner = Vector4.Transform(Vector3.One, invCascadeMat).ScaleToVector3();
                otherCorner = Vector4.Transform(otherCorner, sunLight.GlobalShadowMatrix).ScaleToVector3();

                // Calculate the scale and offset
                var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                sunLight.CascadeOffsets[cascadeIndex] = new Vector4(-cascadeCorner, 0.0f);
                sunLight.CascadeScales[cascadeIndex] = new Vector4(cascadeScale, 1.0f);
            }
        }

        private void RenderLight(
            Sunlight sunlight,
            RenderTarget2D shadowMap,
            RenderTarget2D colorMap,
            PerspectiveCamera perspectiveCamera,
            GBuffer gBuffer)
        {
            // G-Buffer input     
            this.Effect.NormalMap = gBuffer.NormalTarget;
            this.Effect.DepthMap = gBuffer.DepthTarget;

            // Light properties
            this.Effect.SurfaceToLightVector = sunlight.SurfaceToLightVector;
            this.Effect.LightColor = sunlight.Color;

            // Camera properties for specular reflections, and rebuilding world positions
            this.Effect.CameraPosition = perspectiveCamera.Position;
            this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;

            // Shadow properties
            this.Effect.ShadowMap = shadowMap;
            this.Effect.ColorMap = colorMap;
            this.Effect.ShadowMatrix = sunlight.GlobalShadowMatrix;
            this.Effect.CascadeSplits = sunlight.CascadeSplits;
            this.Effect.CascadeOffsets = sunlight.CascadeOffsets;
            this.Effect.CascadeScales = sunlight.CascadeScales;

            this.Effect.Apply();

            this.FullScreenTriangle.Render(this.Device);

        }
    }
}