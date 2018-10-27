using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering.Effects;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using DirectionalLight = MiniEngine.Rendering.Components.DirectionalLight;

namespace MiniEngine.Rendering.Systems
{
    public sealed class DirectionalLightSystem : ISystem
    {
        private readonly GraphicsDevice Device;
        private readonly DirectionalLightEffect Effect;
        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly Dictionary<Entity, DirectionalLight> Lights;

        public DirectionalLightSystem(GraphicsDevice device, DirectionalLightEffect effect)
        {
            this.Device = device;
            this.Effect = effect;
            this.FullScreenTriangle = new FullScreenTriangle();

            this.Lights = new Dictionary<Entity, DirectionalLight>();
        }

        public bool Contains(Entity entity) => this.Lights.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var light = this.Lights[entity];
            return $"directional light, direction: {light.Direction}, color: {light.Color}";
        }

        public void Remove(Entity entity) => this.Lights.Remove(entity);

        public void Add(Entity entity, Vector3 direction, Color color) => this.Lights.Add(entity, new DirectionalLight(direction, color));

        public void Render(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.LightState())
            {
                foreach (var light in this.Lights.Values)
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