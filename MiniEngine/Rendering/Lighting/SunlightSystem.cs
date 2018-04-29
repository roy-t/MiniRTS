using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Lighting.Components;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Scenes;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class SunlightSystem
    {
        public const int Resolution = 2048;
        public const int Cascades = 4;
        private static readonly float[] CascadeDistances = { 0.05f, 0.15f, 0.5f, 1.0f };

        private readonly GraphicsDevice Device;
        private readonly Effect CascadingShadowMapEffect;
        private readonly Effect SunlightEffect;
        private readonly Quad Quad;
        private readonly Frustum Frustum;

        private readonly Dictionary<int, ShadowMap> ShadowMaps;
        private readonly Dictionary<int, Sunlight> Lights;

        public SunlightSystem(GraphicsDevice device, Effect cascadingShadowMapEffect, Effect sunlightEffect)
        {
            this.Device = device;
            this.CascadingShadowMapEffect = cascadingShadowMapEffect;
            this.SunlightEffect = sunlightEffect;
            this.Quad = new Quad();
            this.Frustum = new Frustum();

            this.ShadowMaps = new Dictionary<int, ShadowMap>(1);
            this.Lights = new Dictionary<int, Sunlight>(1);
        }

        public void Compose(int entity, Color color, Vector3 position, Vector3 lookAt)
        {
            this.ShadowMaps.Add(entity, new ShadowMap(this.Device, Resolution, Cascades));
            this.Lights.Add(entity, new Sunlight(color, position, lookAt));
        }

        public void Decompose(int entity)
        {
            this.ShadowMaps.Remove(entity);
            this.Lights.Remove(entity);
        }

        public void RenderShadowMaps(IScene geometry, PerspectiveCamera perspectiveCamera)
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
                    RenderShadowMap(shadowMap, geometry);
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

        private void RenderShadowMap(ShadowMap work, IScene geometry)
        {
            for (var cascadeIndex = 0; cascadeIndex < Cascades; cascadeIndex++)
            {
                // Set the rendertarget and clear it to white (max distance)
                this.Device.SetRenderTarget(work.RenderTarget, cascadeIndex);
                this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                // Draw the geometry, as seen from the shadow camera
                geometry.Draw(this.CascadingShadowMapEffect, work.ShadowCameras[cascadeIndex]);
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
            this.SunlightEffect.Parameters["ColorMap"].SetValue(color);
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