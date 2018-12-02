using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class AmbientLightSystem : ISystem
    {
        private const int KernelSize = 64;

        private readonly GraphicsDevice Device;
        private readonly AmbientOcclusionEffect AmbientOcclusionEffect;
        private readonly AmbientLightEffect AmbientLightEffect;
        private readonly EntityLinker EntityLinker;
        private readonly FullScreenTriangle FullScreenTriangle;
        private readonly List<AmbientLight> Lights;
        private readonly Vector3[] Kernel;
                
        public AmbientLightSystem(GraphicsDevice device, AmbientOcclusionEffect ambientLightEffect, AmbientLightEffect blurEffect, EntityLinker entityLinker)
        {
            this.Device = device;
            this.AmbientOcclusionEffect = ambientLightEffect;
            this.AmbientLightEffect = blurEffect;
            this.EntityLinker = entityLinker;
            this.FullScreenTriangle = new FullScreenTriangle();
            this.Lights = new List<AmbientLight>();                       

            this.Kernel = this.GenerateKernel();
        }

        public void RenderAmbientOcclusion(PerspectiveCamera camera, GBuffer gBuffer)
        {                                
            using(this.Device.LightState())
            {
                // G-Buffer input
                this.AmbientOcclusionEffect.DepthMap = gBuffer.DepthTarget;

                // Light properties                
                this.AmbientOcclusionEffect.Kernel = this.Kernel;

                // Camera properties
                this.AmbientOcclusionEffect.View = camera.View;
                this.AmbientOcclusionEffect.Projection = camera.Projection;
                this.AmbientOcclusionEffect.InverseViewProjection = camera.InverseViewProjection;

                this.AmbientOcclusionEffect.Apply();
                this.FullScreenTriangle.Render(this.Device);
            }
        }

        public void RenderAmbientLight(PerspectiveCamera camera, GBuffer gBuffer)
        {
            var ambientLight = this.ComputeAmbientLightZeroAlpha();

            using (this.Device.LightState())
            {
                // G-Buffer input
                this.AmbientLightEffect.AmbientOcclusionMap = gBuffer.AmbientOcclusionTarget;
                
                // Light properties
                this.AmbientLightEffect.Color = ambientLight;

                this.AmbientLightEffect.Apply();
                this.FullScreenTriangle.Render(this.Device);
            }
        }
               
        private Color ComputeAmbientLightZeroAlpha()
        {   
            this.Lights.Clear();
            this.EntityLinker.GetComponents(this.Lights);

            var accumulate = Color.TransparentBlack;
            foreach (var light in this.Lights)
            {
                accumulate.R += light.Color.R;
                accumulate.G += light.Color.G;
                accumulate.B += light.Color.B;
            }

            return accumulate;
        }
        
        private Vector3[] GenerateKernel()
        {            
            var random = new Random(235);
            var kernel = new Vector3[KernelSize];

            for(var i = 0; i < KernelSize; i++)
            {
                var scale = i / (float)KernelSize;
                var v = new Vector3
                (
                    (2.0f * (float)random.NextDouble()) - 1.0f,
                    (2.0f * (float)random.NextDouble()) - 1.0f,
                    (2.0f * (float)random.NextDouble()) - 1.0f
                );                 

                v *= 0.1f + (0.9f * scale * scale);

                kernel[i] = v;
            }

            return kernel;
        }
    }
}