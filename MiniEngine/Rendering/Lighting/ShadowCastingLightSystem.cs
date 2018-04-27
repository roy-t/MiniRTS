using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Primitives;
using MiniEngine.Scenes;

namespace MiniEngine.Rendering.Lighting
{
    public sealed class ShadowCastingLightSystem
    {
        private readonly GraphicsDevice Device;
        private readonly Effect ShadowMapEffect;
        private readonly Effect ShadowCastingLightEffect;
        private readonly Quad Quad;

        public ShadowCastingLightSystem(GraphicsDevice device, Effect shadowMapEffect, Effect shadowCastingLightEffect)
        {
            this.Device = device;
            this.ShadowMapEffect = shadowMapEffect;
            this.ShadowCastingLightEffect = shadowCastingLightEffect;
            this.Quad = new Quad();
        }

        public void RenderShadowMaps(IEnumerable<ShadowCastingLight> lights, IScene geometry)
        {
            using (this.Device.GeometryState())
            {
                foreach (var light in lights)
                {
                    this.Device.SetRenderTarget(light.ShadowMap);
                    this.Device.Clear(Color.Black);
                    geometry.Draw(this.ShadowMapEffect, light);

                    this.Device.SetRenderTarget(null);
                }
            }
        }

        public void RenderLights(
            IEnumerable<ShadowCastingLight> lights,
            PerspectiveCamera perspectiveCamera,
            RenderTarget2D color,
            RenderTarget2D normal,
            RenderTarget2D depth)
        {
            using (this.Device.LightState())
            {
                foreach (var light in lights)
                {
                    // G-Buffer input                        
                    this.ShadowCastingLightEffect.Parameters["NormalMap"].SetValue(normal);
                    this.ShadowCastingLightEffect.Parameters["DepthMap"].SetValue(depth);

                    // Light properties
                    this.ShadowCastingLightEffect.Parameters["LightDirection"].SetValue(Vector3.Normalize(light.LookAt - light.Position));
                    this.ShadowCastingLightEffect.Parameters["LightPosition"].SetValue(light.Position);
                    this.ShadowCastingLightEffect.Parameters["Color"].SetValue(light.ColorVector);

                    // Camera properties for specular reflections
                    this.ShadowCastingLightEffect.Parameters["CameraPosition"].SetValue(perspectiveCamera.Position);
                    this.ShadowCastingLightEffect.Parameters["InverseViewProjection"].SetValue(perspectiveCamera.InverseViewProjection);

                    // Shadow properties
                    this.ShadowCastingLightEffect.Parameters["ShadowMap"].SetValue(light.ShadowMap);
                    this.ShadowCastingLightEffect.Parameters["LightView"].SetValue(light.View);
                    this.ShadowCastingLightEffect.Parameters["LightProjection"].SetValue(light.Projection);

                    foreach (var pass in this.ShadowCastingLightEffect.Techniques[0].Passes)
                    {                      
                        pass.Apply();
                        this.Quad.Render(this.Device);
                    }
                }                
            }
        }
    }
}
