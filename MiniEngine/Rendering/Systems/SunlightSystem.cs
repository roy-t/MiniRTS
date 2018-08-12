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

        private readonly ModelSystem ModelSystem;

        private readonly GraphicsDevice Device;
        private readonly Effect ShadowMapEffect;
        private readonly Effect SunlightEffect;
        private readonly Quad Quad;
        private readonly Frustum Frustum;

        private readonly Dictionary<Entity, ShadowMap> ShadowMaps;
        private readonly Dictionary<Entity, Sunlight> Lights;

        public SunlightSystem(GraphicsDevice device, Effect shadowMapEffect, Effect sunlightEffect, ModelSystem modelSystem)
        {
            this.Device = device;
            this.ShadowMapEffect = shadowMapEffect;
            this.SunlightEffect = sunlightEffect;
            this.ModelSystem = modelSystem;

            this.Quad = new Quad();
            this.Frustum = new Frustum();

            this.ShadowMaps = new Dictionary<Entity, ShadowMap>(1);
            this.Lights = new Dictionary<Entity, Sunlight>(1);
        }

        public void Add(Entity entity, Color color, Vector3 position, Vector3 lookAt)
        {
            this.ShadowMaps.Add(entity, new ShadowMap(this.Device, Resolution, Cascades));
            this.Lights.Add(entity, new Sunlight(color, position, lookAt));
        }

        public bool Contains(Entity entity) => this.Lights.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var light = this.Lights[entity];
            return $"sun light, direction: {light.LookAt - light.LookAt}, color {light.Color}";
        }

        public void RemoveAll()
        {
            this.ShadowMaps.Clear();
            this.Lights.Clear();
        }

        public void Remove(Entity entity)
        {
            this.ShadowMaps.Remove(entity);
            this.Lights.Remove(entity);
        }        

        public void RenderShadowMaps(PerspectiveCamera perspectiveCamera)
        {
            foreach (var pair in this.Lights)
            {
                var light = pair.Value;
                var shadowMap = this.ShadowMaps[pair.Key];

                ComputeCascades(light, shadowMap, perspectiveCamera);                
            }

            using (this.Device.ShadowMapState())
            {
                foreach (var shadowMap in this.ShadowMaps.Values)
                {
                    RenderShadowMap(shadowMap);
                }               
            }                   
        }

        public void RenderLights(PerspectiveCamera perspectiveCamera, RenderTarget2D color, RenderTarget2D normal, RenderTarget2D depth)
        {
            using (this.Device.SunlightState())
            {
                foreach (var pair in this.Lights)
                {
                    var light = pair.Value;
                    var shadowMap = this.ShadowMaps[pair.Key];
                    RenderLight(light, shadowMap, perspectiveCamera, color, normal, depth);
                }
            }
        }

        private void ComputeCascades(Sunlight light, ShadowMap shadowMap, PerspectiveCamera perspectiveCamera)
        {
            this.Frustum.ResetToViewVolume();
            this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
            shadowMap.GlobalShadowMatrix = ShadowMath.CreateGlobalShadowMatrix(light.SurfaceToLightVector, this.Frustum);

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
                var shadowCamera = ShadowMath.CreateShadowCamera(light.SurfaceToLightVector, this.Frustum, Resolution);
                shadowMap.ShadowCameras[cascadeIndex] = shadowCamera;

                // ViewProjection matrix of the shadow camera that transforms to texture space [0, 1] instead of [-1, 1]
                var shadowMatrix = (shadowCamera.View * shadowCamera.Projection).TextureScaleTransform();

                // Store the split distance in terms of view space depth
                var clipDistance = perspectiveCamera.FarPlane - perspectiveCamera.NearPlane;
                shadowMap.CascadeSplits[cascadeIndex] = perspectiveCamera.NearPlane + farZ * clipDistance;

                // Find scale and offset of this cascade in world space                    
                var invCascadeMat = Matrix.Invert(shadowMatrix);
                var cascadeCorner = Vector4.Transform(Vector3.Zero, invCascadeMat).ScaleToVector3();
                cascadeCorner = Vector4.Transform(cascadeCorner, shadowMap.GlobalShadowMatrix).ScaleToVector3();

                // Do the same for the upper corner
                var otherCorner = Vector4.Transform(Vector3.One, invCascadeMat).ScaleToVector3();
                otherCorner = Vector4.Transform(otherCorner, shadowMap.GlobalShadowMatrix).ScaleToVector3();

                // Calculate the scale and offset
                var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                shadowMap.CascadeOffsets[cascadeIndex] = new Vector4(-cascadeCorner, 0.0f);
                shadowMap.CascadeScales[cascadeIndex] = new Vector4(cascadeScale, 1.0f);
            }
        }

        private void RenderShadowMap(ShadowMap work)
        {
            for (var cascadeIndex = 0; cascadeIndex < Cascades; cascadeIndex++)
            {
                // Set the rendertarget and clear it to white (max distance)
                this.Device.SetRenderTarget(work.RenderTarget, cascadeIndex);
                this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                // Draw the geometry, as seen from the shadow camera
                this.ModelSystem.DrawModels(work.ShadowCameras[cascadeIndex], this.ShadowMapEffect);                
            }
        }        

        private void RenderLight(
            Sunlight light,
            ShadowMap data,
            PerspectiveCamera perspectiveCamera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth)
        {            
            // G-Buffer input                                    
            this.SunlightEffect.Parameters["NormalMap"].SetValue(normal);
            this.SunlightEffect.Parameters["DepthMap"].SetValue(depth);

            // Light properties
            this.SunlightEffect.Parameters["SurfaceToLightVector"].SetValue(light.SurfaceToLightVector);
            this.SunlightEffect.Parameters["LightColor"].SetValue(light.ColorVector);

            // Camera properties for specular reflections, and rebuilding world positions
            this.SunlightEffect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
            this.SunlightEffect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);

            // Shadow properties
            this.SunlightEffect.Parameters["ShadowMap"].SetValue(data.RenderTarget);
            this.SunlightEffect.Parameters["ShadowMatrix"].SetValue(data.GlobalShadowMatrix);
            this.SunlightEffect.Parameters["CascadeSplits"].SetValue(
                new Vector4(
                    data.CascadeSplits[0],
                    data.CascadeSplits[1],
                    data.CascadeSplits[2],
                    data.CascadeSplits[3]));
            this.SunlightEffect.Parameters["CascadeOffsets"].SetValue(data.CascadeOffsets);
            this.SunlightEffect.Parameters["CascadeScales"].SetValue(data.CascadeScales);

            foreach (var pass in this.SunlightEffect.Techniques[0].Passes)
            {
                pass.Apply();
                this.Quad.Render(this.Device);
            }
        }      
    }    
}