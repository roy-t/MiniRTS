using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;
using MiniEngine.Rendering.Foo;
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
        private readonly Vector3[] FrustumCorners;

        public SunlightSystem(GraphicsDevice device, Effect cascadingShadowMapEffect, Effect sunlightEffect)
        {
            this.Device = device;
            this.CascadingShadowMapEffect = cascadingShadowMapEffect;
            this.SunlightEffect = sunlightEffect;
            this.Quad = new Quad();
            this.FrustumCorners = new Vector3[8];
        }

        public void RenderShadowMaps(IEnumerable<Sunlight> lights, IScene geometry, Camera camera)
        {            
            var originalViewport = this.Device.Viewport;
            using (this.Device.ShadowMapState())
            {                
                foreach (var light in lights)
                {
                    light.GlobalShadowMatrix = MakeGlobalShadowMatrix(camera, light.SurfaceToLightDirection);

                    for (var cascadeIndex = 0; cascadeIndex < Sunlight.Cascades; cascadeIndex++)
                    {
                        // Set the rendertarget and clear it to white (max distance)
                        this.Device.SetRenderTarget(light.ShadowMap, cascadeIndex);
                        this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                        // Get the 8 points of the view frustum in world space
                        ResetViewFrustumCorners();

                        // Get the range this cascade covers
                        var prevSplitDist = cascadeIndex == 0 ? 0.0f : light.CascadeSplits[cascadeIndex - 1];
                        var splitDist = light.CascadeSplits[cascadeIndex];

                        var invViewProj = camera.InverseViewProjection;
                        for (var i = 0; i < 8; i++)
                        {
                            this.FrustumCorners[i] = Vector4.Transform(this.FrustumCorners[i], invViewProj).ScaleToVector3();
                        }

                        // Compute the corners of this slice of the frustum by taking a slice
                        // of the identity frustum
                        for (var i = 0; i < 4; i++)
                        {
                            var cornerRay = this.FrustumCorners[i + 4] - this.FrustumCorners[i];
                            var nearCornerRay = cornerRay * prevSplitDist;
                            var farCornerRay = cornerRay * splitDist;
                            this.FrustumCorners[i + 4] = this.FrustumCorners[i] + farCornerRay;
                            this.FrustumCorners[i] = this.FrustumCorners[i] + nearCornerRay;
                        }

                        // Calculate the centroid of the view frustum slice
                        var frustumCenter = Vector3.Zero;
                        for (var i = 0; i < 8; i++)
                        {
                            frustumCenter = frustumCenter + this.FrustumCorners[i];
                        }

                        frustumCenter /= 8.0f;

                        // Calculate the bounding sphere of the view frustum so we can stabilize it later
                        // TODO: can we replace this logic with XNA's bounding sphere?
                        var sphereRadius = 0.0f;
                        for (var i = 0; i < 8; i++)
                        {
                            var dist = (this.FrustumCorners[i] - frustumCenter).Length();
                            sphereRadius = Math.Max(sphereRadius, dist);
                        }

                        // Slightly 'round' it
                        sphereRadius = (float) Math.Ceiling(sphereRadius * 16.0f) / 16.0f;

                        var maxExtents = new Vector3(sphereRadius);
                        var minExtents = -maxExtents;
                        var cascadeExtents = maxExtents - minExtents;

                        // Compute the position of the shadow camera (the position where we're going to capture our shadow maps from)
                        var shadowCameraPos = frustumCenter + light.SurfaceToLightDirection * -minExtents.Z;
                        var shadowCamera = new OrthographicShadowCamera(
                            minExtents.X,
                            minExtents.Y,
                            maxExtents.X,
                            maxExtents.Y,
                            0.0f,
                            cascadeExtents.Z);

                        shadowCamera.SetLookAt(shadowCameraPos, frustumCenter, Vector3.Up);
                        // STABILIZE
                        // Create the rounding matrix, by projecting the world-space origin and determining
                        // the fractional offset in texel space
                        var shadowMatrixTemp = shadowCamera.ViewProjection;
                        var shadowOrigin = new Vector4(0.0f, 0.0f, 0.0f, 1.0f);
                        shadowOrigin = Vector4.Transform(shadowOrigin, shadowMatrixTemp);
                        shadowOrigin = shadowOrigin * (Sunlight.Resolution / 2.0f);

                        var roundedOrigin = shadowOrigin.Round();
                        var roundOffset = roundedOrigin - shadowOrigin;
                        roundOffset = roundOffset * (2.0f / Sunlight.Resolution);
                        roundOffset.Z = 0.0f;
                        roundOffset.W = 0.0f;

                        var shadowProj = shadowCamera.Projection;
                        //shadowProj.r[3] = shadowProj.r[3] + roundOffset;
                        shadowProj.M41 += roundOffset.X;
                        shadowProj.M42 += roundOffset.Y;
                        shadowProj.M43 += roundOffset.Z;
                        shadowProj.M44 += roundOffset.W;
                        shadowCamera.Projection = shadowProj;
                        // STABILIZE



                        geometry.Draw(this.CascadingShadowMapEffect, shadowCamera);                        
                        this.Device.SetRenderTarget(null);

                        // Calculate the matrix which transforms from [-1, 1] to
                        // [0, 1] space.
                        var texScaleBias = Matrix.CreateScale(0.5f, -0.5f, 1.0f) *
                                           Matrix.CreateTranslation(0.5f, 0.5f, 0.0f);

                        var shadowMatrix = shadowCamera.ViewProjection;
                        shadowMatrix = shadowMatrix * texScaleBias;

                        // Store the split distance in terms of view space depth
                        var clipDist = camera.FarPlane - camera.NearPlane;
                        light.CascadeSplitsUV[cascadeIndex] = camera.NearPlane + splitDist * clipDist;

                        // Calculate the position of the lower corner of the cascade partition in the UV space of the 
                        // first cascade partition
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
            }

            this.Device.Viewport = originalViewport;
        }

        private void ResetViewFrustumCorners()
        {
            this.FrustumCorners[0] = new Vector3(-1.0f, 1.0f, 0.0f);
            this.FrustumCorners[1] = new Vector3(1.0f, 1.0f, 0.0f);
            this.FrustumCorners[2] = new Vector3(1.0f, -1.0f, 0.0f);
            this.FrustumCorners[3] = new Vector3(-1.0f, -1.0f, 0.0f);
            this.FrustumCorners[4] = new Vector3(-1.0f, 1.0f, 1.0f);
            this.FrustumCorners[5] = new Vector3(1.0f, 1.0f, 1.0f);
            this.FrustumCorners[6] = new Vector3(1.0f, -1.0f, 1.0f);
            this.FrustumCorners[7] = new Vector3(-1.0f, -1.0f, 1.0f);
        }

        /// <summary>
        /// Makes the "global" shadow matrix used as the reference point for the cascades.
        /// </summary>
        private Matrix MakeGlobalShadowMatrix(Camera camera, Vector3 surfaceToLightDirection)
        {
            ResetViewFrustumCorners();

            var invViewProj = camera.InverseViewProjection;

            var frustumCenter = Vector3.Zero;
            for (var i = 0; i < 8; i++)
            {
                this.FrustumCorners[i] = Vector4.Transform(this.FrustumCorners[i], invViewProj).ScaleToVector3();
                frustumCenter += this.FrustumCorners[i];
            }

            frustumCenter /= 8.0f;

            var shadowCameraPos = frustumCenter + surfaceToLightDirection * -0.5f;
            var shadowCamera = new OrthographicShadowCamera(-0.5f, -0.5f, 0.5f, 0.5f, 0.0f, 1.0f);
            shadowCamera.SetLookAt(shadowCameraPos, frustumCenter, Vector3.Up);

            var texScaleBias = Matrix.CreateScale(0.5f, -0.5f, 1.0f);
            texScaleBias.Translation = new Vector3(0.5f, 0.5f, 0.0f);
            return shadowCamera.ViewProjection * texScaleBias;
        }

        public void RenderLights(
            IEnumerable<Sunlight> lights,
            Camera camera,
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
                    this.SunlightEffect.Parameters["CameraPosition"].SetValue(camera.Position);
                    this.SunlightEffect.Parameters["InverseViewProjection"].SetValue(camera.InverseViewProjection);                    
                    
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
    }
}
