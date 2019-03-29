using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Rendering;
using MiniEngine.Systems;
using MiniEngine.Units;
using System.Linq;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine.UI
{
    public sealed class UIManager
    {
        private readonly Game Game;
        private readonly SpriteBatch SpriteBatch;

        private readonly RenderTargetDescriber RenderTargetDescriber;

        private readonly UIState UIState;
        private readonly ImGuiRenderer gui;

        private KeyboardInput keyboardInput;
        private MouseInput mouseInput;

        private CameraController cameraController;
        private LightsController lightsController;

        private FileMenu fileMenu;
        private EntityMenu entitiesMenu;
        private CreateMenu createMenu;
        private RenderingMenu renderingMenu;
        private DebugMenu debugMenu;
        private EntityWindow entityWindow;

        public UIManager(Game game, SpriteBatch spriteBatch, RenderTargetDescriber renderTargetDescriber, DeferredRenderPipeline renderPipeline, PerspectiveCamera camera, SceneSelector sceneSelector, Injector injector)
        {
            this.Game = game;
            this.SpriteBatch = spriteBatch;

            this.gui = new ImGuiRenderer(game);
            this.gui.RebuildFontAtlas();

            this.RenderTargetDescriber = renderTargetDescriber;

            this.keyboardInput = injector.Resolve<KeyboardInput>();
            this.mouseInput = injector.Resolve<MouseInput>();

            var entityManager = injector.Resolve<EntityManager>();

            var lightsFactory = injector.Resolve<LightsFactory>();
            var outlineFactory = injector.Resolve<OutlineFactory>();

            this.cameraController = new CameraController(this.keyboardInput, this.mouseInput, camera);
            this.lightsController = new LightsController(entityManager, lightsFactory);

            this.UIState = UIState.Deserialize();

            this.fileMenu = new FileMenu(this.UIState, game, sceneSelector);
            this.entitiesMenu = new EntityMenu(this.UIState, entityManager);
            this.createMenu = new CreateMenu(this.UIState, entityManager, outlineFactory, this.lightsController, camera);
            this.debugMenu = new DebugMenu(this.UIState, renderTargetDescriber, game);
            this.entityWindow = new EntityWindow(this.UIState, entityManager);
            this.renderingMenu = new RenderingMenu(this.UIState, renderPipeline);

            camera.Move(this.UIState.EditorState.CameraPosition, this.UIState.EditorState.CameraLookAt);
        }

        public void Render(Viewport viewport, GameTime gameTime)
        {                        
            // Do not handle input if game window is not activated
            if (!this.Game.IsActive)
            {
                return;
            }

            var elapsed = (Seconds)gameTime.ElapsedGameTime;

            this.keyboardInput.Update();
            this.mouseInput.Update();

            this.cameraController.Update(elapsed);

            if (this.keyboardInput.Click(Keys.F12))
            {
                this.UIState.EditorState.ShowGui = !this.UIState.EditorState.ShowGui;
            }

            if (this.keyboardInput.Click(Keys.Escape))
            {
                this.Game.Exit();
            }

            this.RenderOverlay(viewport);
            this.RenderUI(gameTime);
        }

        private void RenderOverlay(Viewport viewport)
        {
            this.SpriteBatch.Begin(
              SpriteSortMode.Deferred,
              BlendState.Opaque,
              SamplerState.LinearClamp,
              DepthStencilState.None,
              RasterizerState.CullCounterClockwise);


            switch (this.UIState.DebugState.DebugDisplay)
            {
                case DebugDisplay.None:
                    break;
                case DebugDisplay.Single:
                    this.SpriteBatch.Draw(
                        this.RenderTargetDescriber.GetRenderTarget(this.UIState.DebugState.SelectedRenderTarget),
                           new Vector2(0, 0),
                           null,
                           Color.White,
                           0.0f,
                           Vector2.Zero,
                           1.0f,
                           SpriteEffects.None,
                           0);
                    break;
                case DebugDisplay.Combined:
                    if (this.UIState.DebugState.SelectedRenderTargets.Any())
                    {
                        var count = this.UIState.DebugState.SelectedRenderTargets.Count;
                        var xStep = viewport.Width / this.UIState.DebugState.Columns;
                        var yStep = xStep / viewport.AspectRatio;

                        var renderTargets = this.RenderTargetDescriber.GetRenderTargets(this.UIState.DebugState.SelectedRenderTargets);
                        for (var i = 0; i < renderTargets.Count; i++)
                        {
                            this.SpriteBatch.Draw(
                                renderTargets[i],
                                new Vector2(xStep * (i % this.UIState.DebugState.Columns), yStep * (i / this.UIState.DebugState.Columns)),
                                null,
                                Color.White,
                                0.0f,
                                Vector2.Zero,
                                1.0f / this.UIState.DebugState.Columns,
                                SpriteEffects.None,
                                0);
                        }
                    }
                    break;
            }

            this.SpriteBatch.End();
        }

        private void RenderUI(GameTime gameTime)
        {
            if (this.UIState.EditorState.ShowGui)
            {
                this.gui.BeginLayout(gameTime);
                {
                    if (ImGui.BeginMainMenuBar())
                    {
                        this.fileMenu.Render();
                        this.entitiesMenu.Render();
                        this.createMenu.Render();
                        this.renderingMenu.Render();
                        this.debugMenu.Render();

                        ImGui.EndMainMenuBar();
                    }

                    if (this.UIState.EntityState.ShowEntityWindow)
                    {
                        this.entityWindow.Render();
                    }

                    if (this.UIState.DebugState.ShowDemo) { ImGui.ShowDemoWindow(); }
                }
                this.gui.EndLayout();
            }
        }

        public void Close(IViewPoint viewPoint) 
            => this.UIState.Serialize(viewPoint.Position, viewPoint.Position + viewPoint.Forward);
    }
}
