using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;
using DirectionalLight = MiniEngine.Rendering.Lighting.DirectionalLight;

namespace MiniEngine.Scenes
{
    public abstract class AScene : IScene
    {
        protected readonly GraphicsDevice Device;

        protected AScene(GraphicsDevice device, Camera camera)
        {
            this.Device = device;
            this.Camera = camera;

            this.DirectionalLights = new List<DirectionalLight>();
            this.PointLights = new List<PointLight>();
        }

        public Camera Camera { get; }

        public List<DirectionalLight> DirectionalLights { get; private set; }        

        public List<PointLight> PointLights { get; private set; }

        public abstract void LoadContent(ContentManager content);

        public abstract void Update(Seconds elapsed);

        public abstract void Draw();

        protected void DrawModel(Model model, Matrix world)
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
