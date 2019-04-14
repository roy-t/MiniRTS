using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Pipeline.Lights.Factories;
using MiniEngine.Pipeline.Models.Factories;
using MiniEngine.Pipeline.Projectors.Factories;
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
        private const float SlowDragSpeed = 0.01f;
        private const float FastDragSpeed = 0.05f;

        private readonly Game Game;
        private readonly SpriteBatch SpriteBatch;

        private readonly RenderTargetDescriber RenderTargetDescriber;

        private readonly UIState UIState;
        private readonly ImGuiRenderer Gui;

        private readonly Editors Editors;

        private readonly KeyboardInput KeyboardInput;
        private readonly MouseInput MouseInput;

        private readonly CameraController CameraController;
        private readonly LightsController LightsController;

        private readonly FileMenu FileMenu;
        private readonly EntityMenu EntitiesMenu;
        private readonly CreateMenu CreateMenu;
        private readonly RenderingMenu RenderingMenu;
        private readonly DebugMenu DebugMenu;
        private readonly EntityWindow EntityWindow;

        public UIManager(Game game, SpriteBatch spriteBatch, RenderTargetDescriber renderTargetDescriber, DeferredRenderPipeline renderPipeline, PerspectiveCamera camera, SceneSelector sceneSelector, Injector injector)
        {
            this.Game = game;
            this.SpriteBatch = spriteBatch;

            this.Gui = new ImGuiRenderer(game);
            this.Gui.RebuildFontAtlas();

            this.Editors = new Editors(this.Gui);

            this.RenderTargetDescriber = renderTargetDescriber;

            this.KeyboardInput = injector.Resolve<KeyboardInput>();
            this.MouseInput = injector.Resolve<MouseInput>();

            var entityManager = injector.Resolve<EntityManager>();

            var lightsFactory = injector.Resolve<LightsFactory>();
            var outlineFactory = injector.Resolve<OutlineFactory>();
            var projectorFactory = injector.Resolve<ProjectorFactory>();

            var texture = game.Content.Load<Texture2D>("Debug");

            this.CameraController = new CameraController(this.KeyboardInput, this.MouseInput, camera);
            this.LightsController = new LightsController(entityManager, lightsFactory);

            this.UIState = UIState.Deserialize();

            // After loading the selected entity might have disappeared
            var selectedEntity = this.UIState.EntityState.SelectedEntity;
            var allEntities = entityManager.Creator.GetAllEntities();
            if (!allEntities.Contains(selectedEntity))
            {
                this.UIState.EntityState.SelectedEntity = allEntities.FirstOrDefault();

            }            

            this.FileMenu = new FileMenu(this.UIState, game, sceneSelector);
            this.EntitiesMenu = new EntityMenu(this.UIState, entityManager);
            this.CreateMenu = new CreateMenu(this.UIState, entityManager, outlineFactory, projectorFactory, texture, this.LightsController, camera);
            this.DebugMenu = new DebugMenu(this.UIState, renderTargetDescriber, game);
            this.EntityWindow = new EntityWindow(this.Editors, this.UIState, entityManager);
            this.RenderingMenu = new RenderingMenu(this.Editors, this.UIState, renderPipeline);

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

            this.KeyboardInput.Update();
            this.MouseInput.Update();            

            this.CameraController.Update(elapsed);

            if (this.KeyboardInput.Click(Keys.F12))
            {
                this.UIState.EditorState.ShowGui = !this.UIState.EditorState.ShowGui;
            }

            if (this.KeyboardInput.Click(Keys.Escape))
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
                if (this.KeyboardInput.Hold(Keys.LeftShift))
                {
                    this.Editors.DragSpeed = FastDragSpeed;
                }
                else
                {
                    this.Editors.DragSpeed = SlowDragSpeed;
                }

                this.Gui.BeginLayout(gameTime);
                {
                    if (ImGui.BeginMainMenuBar())
                    {
                        this.FileMenu.Render();
                        this.EntitiesMenu.Render();
                        this.CreateMenu.Render();
                        this.RenderingMenu.Render();
                        this.DebugMenu.Render();

                        ImGui.EndMainMenuBar();
                    }

                    if (this.UIState.EntityState.ShowEntityWindow)
                    {
                        this.EntityWindow.Render();
                    }

                    if (this.UIState.DebugState.ShowDemo) { ImGui.ShowDemoWindow(); }
                }
                this.Gui.EndLayout();
            }
        }

        public void Close(IViewPoint viewPoint) 
            => this.UIState.Serialize(viewPoint.Position, viewPoint.Position + viewPoint.Forward);
    }
}
