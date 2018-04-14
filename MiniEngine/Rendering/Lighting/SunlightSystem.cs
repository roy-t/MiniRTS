using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Mathematics;
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
            using (this.Device.GeometryState())
            {                
                foreach (var light in lights)
                {

                    light.GlobalShadowMatrix = MakeGlobalShadowMatrix(camera, light.Direction);

                    for (var cascadeIndex = 0; cascadeIndex < Sunlight.Cascades; cascadeIndex++)
                    {
                        this.Device.SetRenderTarget(light.ShadowMap, cascadeIndex);
                        this.Device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1.0f, 0);

                        // Get the 8 points of the view frustum in world space
                        ResetViewFrustumCorners();

                        var prevSplitDist = cascadeIndex == 0 ? 0.0f : light.CascadeSplits[cascadeIndex - 1];
                        var splitDist = light.CascadeSplits[cascadeIndex];

                        var invViewProj = camera.InverseViewProjection;
                        for (var i = 0; i < this.FrustumCorners.Length; i++)
                        {
                            this.FrustumCorners[i] = Vector4.Transform(this.FrustumCorners[i], invViewProj).ToVector3();
                        }

                        // Get the corners of the current cascade slice of the view frustum
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
                        for (var i = 0; i < this.FrustumCorners.Length; i++)
                        {
                            frustumCenter = frustumCenter + this.FrustumCorners[i];
                        }

                        frustumCenter /= 8.0f;

                        var sphereRadius = 0.0f;
                        for (var i = 0; i < this.FrustumCorners.Length; i++)
                        {
                            var dist = (this.FrustumCorners[i] - frustumCenter).Length();
                            sphereRadius = Math.Max(sphereRadius, dist);
                        }

                        sphereRadius = (float) Math.Ceiling(sphereRadius * 16.0f) / 16.0f;

                        var maxExtents = new Vector3(sphereRadius);
                        var minExtents = -maxExtents;
                        var cascadeExtents = maxExtents - minExtents;

                        var shadowCameraPos = frustumCenter + light.Direction * -minExtents.Z;
                        var shadowCamera = new OrthographicCamera(
                            minExtents.X,
                            minExtents.Y,
                            maxExtents.X,
                            maxExtents.Y,
                            0.0f,
                            cascadeExtents.Z);

                        shadowCamera.Move(shadowCameraPos, frustumCenter);
                        shadowCamera.Stabilize(Sunlight.Resolution);

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
                        var cascadeCorner = Vector4.Transform(Vector3.One, invCascadeMat).ToVector3();
                        cascadeCorner = Vector4.Transform(cascadeCorner, light.GlobalShadowMatrix).ToVector3();

                        // Do the same for the upper corner
                        var otherCorner = Vector4.Transform(Vector3.One, invCascadeMat).ToVector3();
                        otherCorner = Vector4.Transform(otherCorner, light.GlobalShadowMatrix).ToVector3();

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
        private Matrix MakeGlobalShadowMatrix(Camera camera, Vector3 lightDirection)
        {
            ResetViewFrustumCorners();

            var invViewProj = camera.InverseViewProjection;

            var frustumCenter = Vector3.Zero;
            for (var i = 0; i < this.FrustumCorners.Length; i++)
            {
                this.FrustumCorners[i] = Vector4.Transform(this.FrustumCorners[i], invViewProj).ToVector3();
                frustumCenter += this.FrustumCorners[i];
            }

            frustumCenter /= 8.0f;

            var shadowCameraPos = frustumCenter + lightDirection * -0.5f;
            var shadowCamera = new OrthographicCamera(-0.5f, -0.5f, 0.5f, 0.5f, 0.0f, 1.0f);
            shadowCamera.Move(shadowCameraPos, frustumCenter);

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
                    this.SunlightEffect.Parameters["LightDirection"].SetValue(light.Direction);
                    this.SunlightEffect.Parameters["LightColor"].SetValue(light.ColorVector);
                    //this.SunlightEffect.Parameters["ViewProjection"].SetValue(camera.ViewProjection);

                    // Camera properties for specular reflections
                    this.SunlightEffect.Parameters["CameraPosition"].SetValue(camera.Position);
                    this.SunlightEffect.Parameters["InverseViewProjection"].SetValue(camera.InverseViewProjection);                    
                    
                    // Shadow properties
                    //this.SunlightEffect.Parameters["ShadowMap"].SetValue(light.ShadowMap);
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
