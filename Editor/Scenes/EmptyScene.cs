﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Units;

namespace MiniEngine.Scenes
{
    public sealed class EmptyScene : IScene
    {
        public string Name => "Empty Scene";
        public TextureCube Skybox { get; }

        public void LoadContent(ContentManager content) { }
        public void RenderUI() { }
        public void Set() { }
        public void Update(PerspectiveCamera camera, Seconds elapsed) { }
    }
}