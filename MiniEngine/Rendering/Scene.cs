using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering.Lighting;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Rendering
{
    public sealed class Scene
    {
        private readonly GraphicsDevice Device;
        private Model lizard;
        private Model ship1;
        private Model ship2;

        public Scene(GraphicsDevice device, Camera camera)
        {
            this.Device = device;
            this.Camera = camera;

            this.DirectionalLights = new List<DirectionalLight>
            {
                new DirectionalLight(Vector3.Forward, Color.Yellow)
            };

            this.PointLights = new List<PointLight>
            {
                new PointLight(Vector3.Up * 5, Color.Purple, 10.0f, 1.0f)
            };
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
