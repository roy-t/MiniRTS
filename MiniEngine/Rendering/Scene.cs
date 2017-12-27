using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Rendering
{
    public sealed class Scene
    {
        private readonly GraphicsDevice Device;
        private Model sphere;

        public Scene(GraphicsDevice device, Camera camera)
        {
            this.Device = device;
            this.Camera = camera;

            this.DirectionalLights = new List<DirectionalLight>
            {
                new DirectionalLight(Vector3.Forward, Color.Yellow)
            };
        }

        public Camera Camera { get; }

        public List<DirectionalLight> DirectionalLights { get; }

        public void LoadContent(ContentManager content)
        {
            this.sphere = content.Load<Model>(@"Lizard\Lizard");
        }

        public void Draw()
        {
            using (this.Device.GeometryState())
            {                
                foreach (var mesh in this.sphere.Meshes)
                {
                    foreach (var effect in mesh.Effects)
                    {
                        effect.Parameters["World"].SetValue(
                            Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateScale(0.05f));
                        effect.Parameters["View"].SetValue(this.Camera.View);
                        effect.Parameters["Projection"].SetValue(this.Camera.Projection);
                        effect.Parameters["FarPlane"].SetValue(this.Camera.FarPlane);
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
