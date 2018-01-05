using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public interface IScene
    {
        Camera Camera { get; }
        List<DirectionalLight> DirectionalLights { get; }
        List<PointLight> PointLights { get; }

        void Draw();
        void LoadContent(ContentManager content);
        void Update(Seconds elapsed);
    }
}