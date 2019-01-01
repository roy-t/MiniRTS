using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Primitives;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Systems;
using System.Collections.Generic;
using DirectionalLight = MiniEngine.Pipeline.Lights.Components.DirectionalLight;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class DirectionalLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly DirectionalLightEffect Effect;
        private readonly EntityLinker EntityLinker;
        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly List<DirectionalLight> Lights;

        public DirectionalLightSystem(GraphicsDevice device, DirectionalLightEffect effect, EntityLinker entityLinker)
        {
            this.Device = device;
            this.Effect = effect;
            this.EntityLinker = entityLinker;
            this.FullScreenTriangle = new FullScreenTriangle();
            this.Lights = new List<DirectionalLight>();
        }

        public void Render(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            this.Lights.Clear();
            this.EntityLinker.GetComponents(this.Lights);

            using (this.Device.LightState())
            {
                foreach (var light in this.Lights)
                {
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
}