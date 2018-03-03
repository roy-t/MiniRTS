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

        protected AScene(GraphicsDevice device)
        {
            this.Device = device;            

            this.DirectionalLights = new List<DirectionalLight>();
            this.PointLights = new List<PointLight>();
            this.ShadowCastingLights = new List<ShadowCastingLight>();
            this.AmbientLight = Color.Black;
        }        

        public List<DirectionalLight> DirectionalLights { get; }        

        public List<PointLight> PointLights { get; }

        public List<ShadowCastingLight> ShadowCastingLights { get; }

        public Color AmbientLight { get; protected set; }

        public abstract void LoadContent(ContentManager content);

        public abstract void Update(Seconds elapsed);

        public abstract void Draw(IViewPoint viewPoint);
        public abstract void Draw(Effect effectOverride, IViewPoint viewPoint);

        protected void DrawModel(Model model, Matrix world, IViewPoint viewPoint)
        {
            foreach (var mesh in model.Meshes)
            {
                foreach (var effect in mesh.Effects)
                {
                    effect.Parameters["World"].SetValue(world);
                    effect.Parameters["View"].SetValue(viewPoint.View);
                    effect.Parameters["Projection"].SetValue(viewPoint.Projection);
                }

                mesh.Draw();
            }
        }

        protected void DrawModel(Effect effectOverride, Model model, Matrix world, IViewPoint viewpoint)
        {
            effectOverride.Parameters["World"].SetValue(world);
            effectOverride.Parameters["View"].SetValue(viewpoint.View);
            effectOverride.Parameters["Projection"].SetValue(viewpoint.Projection);

            foreach (var mesh in model.Meshes)
            {
                var effects = new Effect[mesh.MeshParts.Count];

                for (var i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];
                    effects[i] = part.Effect;
                    part.Effect = effectOverride;

                }

                mesh.Draw();

                for (var i = 0; i < mesh.MeshParts.Count; i++)
                {
                    var part = mesh.MeshParts[i];
                    part.Effect = effects[i];
                }
            }
        }    
    }
}
