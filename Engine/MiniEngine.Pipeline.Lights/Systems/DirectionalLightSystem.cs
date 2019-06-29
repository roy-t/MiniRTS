using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using MiniEngine.Systems.Containers;
using DirectionalLight = MiniEngine.Pipeline.Lights.Components.DirectionalLight;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class DirectionalLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly DirectionalLightEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly IComponentContainer<DirectionalLight> Lights;

        public DirectionalLightSystem(GraphicsDevice device, DirectionalLightEffect effect, IComponentContainer<DirectionalLight> lights)
        {
            this.Device = device;
            this.Effect = effect;
            this.Lights = lights;
            this.FullScreenTriangle = new FullScreenTriangle();
            
        }

        public void Render(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            this.Device.LightState();

            for (var i = 0; i < this.Lights.Count; i++)
            {
                var light = this.Lights[i];

                // G-Buffer input                        
                this.Effect.NormalMap = gBuffer.NormalTarget;
                this.Effect.DepthMap = gBuffer.DepthTarget;

                // Light properties
                this.Effect.LightDirection = light.Direction;
                this.Effect.Color = light.Color;

                // Camera properties for specular reflections
                this.Effect.CameraPosition = perspectiveCamera.Position;
                this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;

                this.Effect.Apply();

                this.FullScreenTriangle.Render(this.Device);
            }
        }
    }
}