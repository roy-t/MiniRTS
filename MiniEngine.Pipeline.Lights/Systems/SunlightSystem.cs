using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Primitives;
using MiniEngine.Systems;
using MiniEngine.Effects;
using MiniEngine.Effects.DeviceStates;
using MiniEngine.Pipeline.Lights.Components;
using MiniEngine.Pipeline.Shadows.Systems;

namespace MiniEngine.Pipeline.Lights.Systems
{
    public sealed class SunlightSystem : ISystem
    {
        public const int Resolution = 2048;
        public const int Cascades = 4;       

        private readonly GraphicsDevice Device;
        private readonly Frustum Frustum;
        private readonly FullScreenTriangle FullScreenTriangle;

        private readonly CascadedShadowMapSystem CascadedShadowMapSystem;
        private readonly SunlightEffect Effect;

        private readonly Dictionary<Entity, Sunlight> Sunlights;

        public SunlightSystem(GraphicsDevice device, SunlightEffect effect, CascadedShadowMapSystem cascadedShadowMapSystem)
        {
            this.Device = device;
            this.Effect = effect;
            this.CascadedShadowMapSystem = cascadedShadowMapSystem;

            this.FullScreenTriangle = new FullScreenTriangle();
            this.Frustum = new Frustum();

            this.Sunlights = new Dictionary<Entity, Sunlight>(1);
        }

        public bool Contains(Entity entity) => this.Sunlights.ContainsKey(entity);

        public string Describe(Entity entity)
        {
            var light = this.Sunlights[entity];
            return $"sun light, color {light.Color}";
        }

        public void Remove(Entity entity)
        {
            this.Sunlights.Remove(entity);
            this.CascadedShadowMapSystem.Remove(entity);
        }       

        public void Add(Entity entity, Color color, Vector3 position, Vector3 lookAt)
        {
            var sunlight = new Sunlight(color, Cascades);
            this.Sunlights.Add(entity, sunlight);         

            this.CascadedShadowMapSystem.Add(entity, position, lookAt, Cascades, Resolution);               
        }

        public void RemoveAll()
        {
            var keys = new Entity[this.Sunlights.Keys.Count];
            this.Sunlights.Keys.CopyTo(keys, 0);

            foreach (var key in keys)
            {
                this.Remove(key);
            }
        }

        public void RenderLights(PerspectiveCamera perspectiveCamera, GBuffer gBuffer)
        {
            using (this.Device.ShadowCastingLightState())
            {
                foreach (var pair in this.Sunlights)
                {
                    var entity = pair.Key;
                    var light = pair.Value;

                    var maps = this.CascadedShadowMapSystem.GetMaps(entity);
                    var cascades = this.CascadedShadowMapSystem.GetCascades(entity);

                    // G-Buffer input     
                    this.Effect.NormalMap = gBuffer.NormalTarget;
                    this.Effect.DepthMap = gBuffer.DepthTarget;

                    // Light properties
                    this.Effect.SurfaceToLightVector = cascades.SurfaceToLightVector;
                    this.Effect.LightColor = light.Color;

                    // Camera properties for specular reflections, and rebuilding world positions
                    this.Effect.CameraPosition = perspectiveCamera.Position;
                    this.Effect.InverseViewProjection = perspectiveCamera.InverseViewProjection;

                    // Shadow properties
                    this.Effect.ShadowMap = maps.DepthMapArray;
                    this.Effect.ColorMap = maps.ColorMapArray;
                    this.Effect.ShadowMatrix = cascades.GlobalShadowMatrix;
                    this.Effect.CascadeSplits = cascades.CascadeSplits;
                    this.Effect.CascadeOffsets = cascades.CascadeOffsets;
                    this.Effect.CascadeScales = cascades.CascadeScales;

                    this.Effect.Apply();

                    this.FullScreenTriangle.Render(this.Device);
                }
            }
        }             
    }
}