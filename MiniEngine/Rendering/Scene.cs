using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Rendering
{
    public sealed class Scene
    {
        private readonly Color[] ColorWheel = new[]
        {
            new Color(255, 0, 0),
            new Color(255, 128, 0),
            new Color(255, 255, 0),
            new Color(128, 255, 0),
            new Color(0, 255, 0),
            new Color(0, 255, 128),
            new Color(0, 255, 255),
            new Color(0, 128, 255),
            new Color(0, 0, 255)
        };

        private readonly GraphicsDevice Device;
        private Model lizard;
        private Model ship1;
        private Model ship2;

        private Seconds totalElapsed;

        public Scene(GraphicsDevice device, Camera camera)
        {
            this.Device = device;
            this.Camera = camera;

            this.DirectionalLights = new List<DirectionalLight>
            {
                new DirectionalLight(Vector3.Normalize(Vector3.Forward + Vector3.Down), Color.White * 0.75f),                
                new DirectionalLight(Vector3.Normalize(Vector3.Forward + Vector3.Up + Vector3.Left), Color.BlueViolet * 0.25f)
            };

            this.PointLights = new List<PointLight>();
            foreach (var color in this.ColorWheel)
            {
                this.PointLights.Add(new PointLight(Vector3.Zero, color, 120, 1.0f));
            }

            this.totalElapsed = 0;            
        }

        public Camera Camera { get; }

        public List<DirectionalLight> DirectionalLights { get; }
        public List<PointLight> PointLights { get; }

        public void LoadContent(ContentManager content)
        {
            this.lizard = content.Load<Model>(@"Lizard\Lizard");
            this.ship1 = content.Load<Model>(@"Ship1\Ship1");
            this.ship2 = content.Load<Model>(@"Ship2\Ship2");
        }

        public void Update(Seconds elapsed)
        {
            var step = MathHelper.TwoPi / this.PointLights.Count;
            for (var i = 0; i < this.PointLights.Count; i++)
            {
                var x = Math.Sin(step * i + this.totalElapsed.Value) * 100;
                var y = Math.Cos(step * i + this.totalElapsed.Value) * 100;

                this.PointLights[i].Position = new Vector3((float)x, (float)y, 0);
            }

            this.totalElapsed += elapsed;
        }

        public void Draw()
        {
            using (this.Device.GeometryState())
            {                
                DrawModel(this.ship1, Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.5f));
                DrawModel(this.lizard, Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.05f) * Matrix.CreateTranslation(Vector3.Left * 50));
                DrawModel(this.ship2, Matrix.CreateRotationX(-MathHelper.PiOver2) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Right * 50));
            }
        }

        private void DrawModel(Model model, Matrix world)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(this.Camera.View);
                    effect.Parameters["Projection"].SetValue(this.Camera.Projection);                    
                }

                mesh.Draw();
            }
        }
    }
}
