using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public SunlightSystem(GraphicsDevice device, Effect cascadingShadowMapEffect, Effect sunlightEffect)
        {
            this.Device = device;
            this.CascadingShadowMapEffect = cascadingShadowMapEffect;
            this.SunlightEffect = sunlightEffect;
            this.Quad = new Quad();
        }

        public void RenderShadowMaps(IEnumerable<Sunlight> lights, IScene geometry)
        {
            using (this.Device.GeometryState())
            {
                foreach (var light in lights)
                {
                    this.Device.SetRenderTarget(light.ShadowMap);
                    this.Device.Clear(Color.Black);
                    geometry.Draw(this.CascadingShadowMapEffect, light);

                    this.Device.SetRenderTarget(null);
                }
            }
        }

        public void RenderLights(
            IEnumerable<Sunlight> lights,
            Camera camera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth)
        {
            using (this.Device.LightState())
            {
                foreach (var light in lights)
                {
                    foreach (var pass in this.SunlightEffect.Techniques[0].Passes)
                    {
                        // G-Buffer input                        
                        this.SunlightEffect.Parameters["NormalMap"].SetValue(normal);
                        this.SunlightEffect.Parameters["DepthMap"].SetValue(depth);

                        // Light properties
                        this.SunlightEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(light.LookAt - light.Position));
                        this.SunlightEffect.Parameters["LightPosition"].SetValue(light.Position);
                        this.SunlightEffect.Parameters["Color"].SetValue(light.ColorVector);

                        // Camera properties for specular reflections
                        this.SunlightEffect.Parameters["CameraPosition"].SetValue(camera.Position);
                        this.SunlightEffect.Parameters["InverseViewProjection"].SetValue(camera.InverseViewProjection);

                        // Shadow properties
                        this.SunlightEffect.Parameters["ShadowMap"].SetValue(light.ShadowMap);
                        this.SunlightEffect.Parameters["LightView"].SetValue(light.View);
                        this.SunlightEffect.Parameters["LightProjection"].SetValue(light.Projection);

                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }
                }

                this.Device.SetRenderTarget(null);
            }
        }
    }
}
