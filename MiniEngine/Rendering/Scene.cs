using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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
        }

        public Camera Camera { get; }

        public void LoadContent(ContentManager content)
        {
            this.sphere = content.Load<Model>("sphere");
        }

        public void Draw()
        {
            foreach (var mesh in this.sphere.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    effect.Parameters["WorldViewProj"].SetValue(Matrix.Identity * this.Camera.View * this.Camera.Projection);                    
                }

                mesh.Draw();
            }
        }
    }
}
