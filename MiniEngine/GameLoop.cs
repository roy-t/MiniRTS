using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Rendering;
using MiniEngine.Rendering.Cameras;
using MiniEngine.Rendering.Lighting;
using MiniEngine.Scenes;
using MiniEngine.Utilities;

namespace MiniEngine
{    
    public class GameLoop : Game
    {
        private readonly GraphicsDeviceManager Graphics;
        private readonly KeyboardInput KeyboardInput;
        private readonly MouseInput MouseInput;

        private bool detailView = true;
        private int viewIndex = 0;
        private int viewOptions = 4;

        private PerspectiveCamera perspectiveCamera;
        private SpriteBatch spriteBatch;
        private IScene[] scenes;
        private int currentSceneIndex = 0;
        private CameraController cameraController;
        private RenderSystem renderSystem;

        private SystemCollection systemCollection;
        

        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1080,
                PreferredBackBufferHeight = 768,
                SynchronizeWithVerticalRetrace = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };            

            this.KeyboardInput = new KeyboardInput();
            this.MouseInput = new MouseInput();

            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }
        
        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            this.spriteBatch = new SpriteBatch(this.GraphicsDevice);
            
            this.perspectiveCamera = new PerspectiveCamera(this.GraphicsDevice.Viewport);
            this.cameraController = new CameraController(this.KeyboardInput, this.MouseInput, this.perspectiveCamera);

            this.scenes = new IScene[]
            {                
                new SponzaScene(),
                new ZimaScene()
            };

            this.renderSystem = new RenderSystem(this.GraphicsDevice, this.Content, this.scenes[0]);

            this.systemCollection = new SystemCollection(this.renderSystem.SunlightSystem);

            foreach (var scene in this.scenes)
            {
                scene.LoadContent(this.Content, this.systemCollection);
            }
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
                || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            this.KeyboardInput.Update();
            this.MouseInput.Update();

            if (this.KeyboardInput.Click(Keys.OemTilde))
            {
                this.detailView = !this.detailView;
            }

            if (this.KeyboardInput.Click(Keys.Tab))
            {
                this.currentSceneIndex = (this.currentSceneIndex + 1) % this.scenes.Length;
                this.renderSystem.Scene = this.scenes[this.currentSceneIndex];
            }

            if (this.KeyboardInput.Click(Keys.F))
            {
                this.IsFixedTimeStep = !this.IsFixedTimeStep;
            }

            if (this.KeyboardInput.Click(Keys.OemPlus))
            {
                this.viewIndex = (this.viewIndex + 1) % this.viewOptions;
            }
            else if (this.KeyboardInput.Click(Keys.OemMinus))
            {
                this.viewIndex = (this.viewIndex + this.viewOptions - 1) % this.viewOptions;
            }

            if (this.KeyboardInput.Click(Keys.LeftControl))
            {
                this.renderSystem.EnableFXAA = !this.renderSystem.EnableFXAA;                
            }

            // HACK: dropping some lights
            var selectedScene = this.scenes[this.currentSceneIndex];
            if (this.KeyboardInput.Click(Keys.Q))
            {
                var color = ColorUtilities.PickRandomColor();
                var light = new PointLight(this.perspectiveCamera.Position, color, 10, 1.0f);
                selectedScene.PointLights.Add(light);
            }

            if (this.KeyboardInput.Click(Keys.LeftAlt))
            {
                var light = new ShadowCastingLight(this.GraphicsDevice, this.perspectiveCamera.Position, this.perspectiveCamera.LookAt, Color.White);
                selectedScene.ShadowCastingLights.Add(light);
            }

            if (this.KeyboardInput.Click(Keys.H))
            {
                //selectedScene.Sunlights.ForEach(x => x.Move(this.perspectiveCamera.Position, this.perspectiveCamera.LookAt));
            }

            this.cameraController.Update(gameTime.ElapsedGameTime);

            this.renderSystem.Scene.Update(gameTime.ElapsedGameTime);

            base.Update(gameTime);
        }        

        protected override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.renderSystem.Render(this.perspectiveCamera);
            this.Window.Title = $"{gameTime.ElapsedGameTime.TotalMilliseconds:F2}ms, {(1.0f / gameTime.ElapsedGameTime.TotalSeconds):F2} fps, Fixed Time Step: {this.IsFixedTimeStep} (press 'F' so switch), Camera Position {this.perspectiveCamera.Position}";

            this.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.Opaque,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise);

            var gBuffer = this.renderSystem.GetIntermediateRenderTargets();
            this.viewOptions = gBuffer.Length;

            if (this.detailView)
            {
                var step = this.GraphicsDevice.Viewport.Width / gBuffer.Length;
                for (var i = 0; i < gBuffer.Length; i++)
                {
                    var target = gBuffer[i];
                    this.spriteBatch.Draw(
                        target,
                        new Vector2(step * i, 0),
                        null,
                        Color.White,
                        0.0f,
                        Vector2.Zero,
                        1.0f / gBuffer.Length,
                        SpriteEffects.None,
                        0);
                }
            }
            else
            {
                this.spriteBatch.Draw(
                    gBuffer[this.viewIndex],
                    new Vector2(0, 0),
                    null,
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0);
            }

            this.spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
