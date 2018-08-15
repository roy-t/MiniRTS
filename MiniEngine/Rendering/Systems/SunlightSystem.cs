using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Components;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Systems;
using MiniEngine.Utilities.Extensions;
using System.Collections.Generic;

namespace MiniEngine.Rendering.Systems
{
    public sealed class SunlightSystem : ISystem
    {
        public const int Resolution = 2048;
        public const int Cascades = 4;
        private static readonly float[] CascadeDistances = { 0.05f, 0.15f, 0.5f, 1.0f };

        private readonly ShadowMapSystem ShadowMapSystem;

        private readonly GraphicsDevice Device;        
        private readonly Effect SunlightEffect;
        private readonly Quad Quad;
        private readonly Frustum Frustum;

        private readonly Dictionary<Entity, Sunlight> Sunlights;

        public SunlightSystem(GraphicsDevice device, Effect sunlightEffect, ShadowMapSystem shadowMapSystem)
        {
            this.Device = device;            
            this.SunlightEffect = sunlightEffect;
            this.ShadowMapSystem = shadowMapSystem;

            this.Quad = new Quad();
            this.Frustum = new Frustum();

            this.Sunlights = new Dictionary<Entity, Sunlight>(1);
        }

        public void Add(Entity entity, Color color, Vector3 position, Vector3 lookAt)
        {
            var sunlight = new Sunlight(color, position, lookAt, Cascades);

            this.ShadowMapSystem.Add(entity, sunlight.ShadowCameras, Cascades, Resolution);
            this.Sunlights.Add(entity, sunlight);
        }

        public bool Contains(Entity entity) => this.Sunlights.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var light = this.Sunlights[entity];
            return $"sun light, direction: {light.LookAt - light.LookAt}, color {light.Color}";
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

        public void Remove(Entity entity)
        {
            this.Sunlights.Remove(entity);
            this.ShadowMapSystem.Remove(entity);
        }

        public void Update(PerspectiveCamera perspectiveCamera)
        {
            foreach (var sunLight in this.Sunlights.Values)
            {
                ComputeCascades(sunLight, perspectiveCamera);
            }
        }        

        public void RenderLights(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.SunlightState())
            {
                foreach (var pair in this.Sunlights)
                {
                    var light = pair.Value;
                    var shadowMap = this.ShadowMapSystem.Get(pair.Key).DepthMap;
                    RenderLight(light, shadowMap, perspectiveCamera, gBuffer);
                }
            }
        }

        private void ComputeCascades(Sunlight sunLight, PerspectiveCamera perspectiveCamera)
        {
            this.Frustum.ResetToViewVolume();
            this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
            sunLight.GlobalShadowMatrix = ShadowMath.CreateGlobalShadowMatrix(sunLight.SurfaceToLightVector, this.Frustum);

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
                var shadowCamera = ShadowMath.CreateShadowCamera(sunLight.SurfaceToLightVector, this.Frustum, Resolution);
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

        private void RenderLight(Sunlight sunlight, RenderTarget2D shadowMap, PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {            
            // G-Buffer input                                    
            this.SunlightEffect.Parameters["NormalMap"].SetValue(gBuffer.NormalTarget);
            this.SunlightEffect.Parameters["DepthMap"].SetValue(gBuffer.DepthTarget);

            // Light properties
            this.SunlightEffect.Parameters["SurfaceToLightVector"].SetValue(sunlight.SurfaceToLightVector);
            this.SunlightEffect.Parameters["LightColor"].SetValue(sunlight.ColorVector);

            // Camera properties for specular reflections, and rebuilding world positions
            this.SunlightEffect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
            this.SunlightEffect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);

            // Shadow properties
            this.SunlightEffect.Parameters["ShadowMap"].SetValue(shadowMap);
            this.SunlightEffect.Parameters["ShadowMatrix"].SetValue(sunlight.GlobalShadowMatrix);
            this.SunlightEffect.Parameters["CascadeSplits"].SetValue(
                new Vector4(
                    sunlight.CascadeSplits[0],
                    sunlight.CascadeSplits[1],
                    sunlight.CascadeSplits[2],
                    sunlight.CascadeSplits[3]));
            this.SunlightEffect.Parameters["CascadeOffsets"].SetValue(sunlight.CascadeOffsets);
            this.SunlightEffect.Parameters["CascadeScales"].SetValue(sunlight.CascadeScales);

            foreach (var pass in this.SunlightEffect.Techniques[0].Passes)
            {
                pass.Apply();
                this.Quad.Render(this.Device);
            }
        }      
    }    
}