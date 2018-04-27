using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Scenes;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class SunlightSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect CascadingShadowMapEffect;
        private readonly Effect SunlightEffect;
        private readonly Quad Quad;

        // Stuff to work with
        private readonly Frustum Frustum;

        public SunlightSystem(GraphicsDevice device, Effect cascadingShadowMapEffect, Effect sunlightEffect)
        {
            this.Device = device;
            this.CascadingShadowMapEffect = cascadingShadowMapEffect;
            this.SunlightEffect = sunlightEffect;
            this.Quad = new Quad();
            this.Frustum = new Frustum();
        }

        public void RenderShadowMaps(IReadOnlyList<Sunlight> lights, IScene geometry, PerspectiveCamera perspectiveCamera)
        {
            foreach (var light in lights)
            {                
                this.Frustum.ResetToViewVolume();
                this.Frustum.Transform(perspectiveCamera.InverseViewProjection);
                light.GlobalShadowMatrix = CreateGlobalShadowMatrix(light.SurfaceToLightVector, this.Frustum);

                for (var cascadeIndex = 0; cascadeIndex < Sunlight.Cascades; cascadeIndex++)
                {
                    this.Frustum.ResetToViewVolume();
                    // Transform to world space
                    this.Frustum.Transform(perspectiveCamera.InverseViewProjection);

                    // Slice the frustum
                    var nearZ = cascadeIndex == 0 ? 0.0f : light.CascadeSplits[cascadeIndex - 1];
                    var farZ = light.CascadeSplits[cascadeIndex];
                    this.Frustum.Slice(nearZ, farZ);

                    // Compute the shadow camera, a camera that sits on the surface of the bounding sphere
                    // looking in the direction of the light
                    var shadowCamera = CreateShadowCamera(light.SurfaceToLightVector, this.Frustum);
                    light.ShadowCameras[cascadeIndex] = new ViewPoint(shadowCamera.View, shadowCamera.Projection);

                    // ViewProjection matrix of the shadow camera that transforms to texture space [0, 1] instead of [-1, 1]
                    var shadowMatrix = (shadowCamera.View * shadowCamera.Projection).TextureScaleTransform();

                    // Store the split distance in terms of view space depth
                    var clipDistance = perspectiveCamera.FarPlane - perspectiveCamera.NearPlane;
                    light.CascadeSplitsUV[cascadeIndex] = perspectiveCamera.NearPlane + farZ * clipDistance;

                    // Find scale and offset of this cascade in world space                    
                    var invCascadeMat = Matrix.Invert(shadowMatrix);
                    var cascadeCorner = Vector4.Transform(Vector3.Zero, invCascadeMat).ScaleToVector3();
                    cascadeCorner = Vector4.Transform(cascadeCorner, light.GlobalShadowMatrix).ScaleToVector3();

                    // Do the same for the upper corner
                    var otherCorner = Vector4.Transform(Vector3.One, invCascadeMat).ScaleToVector3();
                    otherCorner = Vector4.Transform(otherCorner, light.GlobalShadowMatrix).ScaleToVector3();

                    // Calculate the scale and offset
                    var cascadeScale = Vector3.One / (otherCorner - cascadeCorner);
                    light.CascadeOffsets[cascadeIndex] = new Vector4(-cascadeCorner, 0.0f);
                    light.CascadeScales[cascadeIndex] = new Vector4(cascadeScale, 1.0f);
                }
            }

            using (this.Device.ShadowMapState())
            {
                foreach (var light in lights)
                {
                    for (var cascadeIndex = 0; cascadeIndex < Sunlight.Cascades; cascadeIndex++)
                    {
                        // Set the rendertarget and clear it to white (max distance)
                        this.Device.SetRenderTarget(light.ShadowMap, cascadeIndex);
                        this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                        // Draw the geometry, as seen from the shadow camera
                        geometry.Draw(this.CascadingShadowMapEffect, light.ShadowCameras[cascadeIndex]);                        
                    }
                }

                this.Device.SetRenderTarget(null);
            }
        }       
        
        public void RenderLights(
            IReadOnlyList<Sunlight> lights,
            PerspectiveCamera perspectiveCamera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth)
        {            
            using (this.Device.SunlightState())
            {                
                foreach (var light in lights)
                {
                    // G-Buffer input                        
                    this.SunlightEffect.Parameters["ColorMap"].SetValue(color);
                    this.SunlightEffect.Parameters["NormalMap"].SetValue(normal);
                    this.SunlightEffect.Parameters["DepthMap"].SetValue(depth);

                    // Light properties
                    this.SunlightEffect.Parameters["LightDirection"].SetValue(light.LightToSurfaceDirection);
                    this.SunlightEffect.Parameters["LightColor"].SetValue(light.ColorVector);

                    // Camera properties for specular reflections, and rebuilding world positions
                    this.SunlightEffect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
                    this.SunlightEffect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);                    
                    
                    // Shadow properties
                    this.SunlightEffect.Parameters["ShadowMap"].SetValue(light.ShadowMap);
                    this.SunlightEffect.Parameters["ShadowMatrix"].SetValue(light.GlobalShadowMatrix);
                    this.SunlightEffect.Parameters["CascadeSplits"].SetValue(
                        new Vector4(
                            light.CascadeSplitsUV[0],
                            light.CascadeSplitsUV[1],
                            light.CascadeSplitsUV[2],
                            light.CascadeSplitsUV[3]));                    
                    this.SunlightEffect.Parameters["CascadeOffsets"].SetValue(light.CascadeOffsets);
                    this.SunlightEffect.Parameters["CascadeScales"].SetValue(light.CascadeScales);

                    foreach (var pass in this.SunlightEffect.Techniques[0].Passes)
                    {                        
                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }
                }                
            }
        }

        /// <summary>
        /// Create the shadow matrix that covers the entire frustum
        /// </summary>
        private static Matrix CreateGlobalShadowMatrix(Vector3 surfaceToLightDirection, Frustum frustum)
        {
            var frustumCenter = frustum.ComputeCenter();

            var shadowCameraPos = frustumCenter + surfaceToLightDirection * -0.5f;
            var shadowCamera = new OrthographicCamera(-0.5f, 0.5f, -0.5f, 0.5f, 0.0f, 1.0f);
            shadowCamera.Move(shadowCameraPos, frustumCenter);

            return (shadowCamera.View * shadowCamera.Projection).TextureScaleTransform();
        }


        private static ViewPoint CreateShadowCamera(Vector3 surfaceToLightVector, Frustum frustum)
        {
            // Compute the bounding sphere of the frustum slice, round the center a little bit
            // so our shadows are more stable when moving the camera around
            var bounds = frustum.ComputeBounds();
            var radius = (float)Math.Ceiling(bounds.Radius * 16.0f) / 16.0f;

            var shadowCamera = new OrthographicCamera(
                -radius,
                radius,
                -radius,
                radius,
                0.0f,
                radius * 2);

            // Compute the position of the shadow camera (the position where we're going to capture our shadow maps from)
            var shadowCameraPos = bounds.Center + surfaceToLightVector * radius;

            shadowCamera.Move(shadowCameraPos, bounds.Center);
            return Stabilize(shadowCamera);
        }      

        private static ViewPoint Stabilize(IViewPoint viewPoint)
        {
            // Create the rounding matrix, by projecting the world-space origin and determining
            // the fractional offset in texel space
            var shadowMatrixTemp = viewPoint.View * viewPoint.Projection;
            var shadowOrigin = new Vector3(0.0f, 0.0f, 0.0f);
            shadowOrigin = Vector3.Transform(shadowOrigin, shadowMatrixTemp);
            shadowOrigin = shadowOrigin * (Sunlight.Resolution / 2.0f);

            var roundedOrigin = shadowOrigin.Round();
            var roundOffset = roundedOrigin - shadowOrigin;
            roundOffset = roundOffset * (2.0f / Sunlight.Resolution);
            roundOffset.Z = 0.0f;

            var shadowProj = viewPoint.Projection;
            shadowProj.M41 += roundOffset.X;
            shadowProj.M42 += roundOffset.Y;
            shadowProj.M43 += roundOffset.Z;

            return new ViewPoint(viewPoint.View, shadowProj);
        }
    }
}