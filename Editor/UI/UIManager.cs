using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Controllers;
using MiniEngine.Input;
using MiniEngine.Primitives.Cameras;
using MiniEngine.Scenes;
using MiniEngine.Systems;
using MiniEngine.UI.State;
using MiniEngine.UI.Utilities;
using MiniEngine.Units;
using KeyboardInput = MiniEngine.Input.KeyboardInput;

namespace MiniEngine.UI
{
    public sealed class UIManager
    {
        private const float SlowDragSpeed = 0.01f;
        private const float FastDragSpeed = 0.05f;

        private readonly Game GameLoop;
        private readonly SpriteBatch SpriteBatch;

        private readonly RenderTargetDescriber RenderTargetDescriber;

        private readonly UIState State;
        private readonly ImGuiRenderer Gui;

        private readonly Editors Editors;

        private readonly KeyboardInput KeyboardInput;
        private readonly MouseInput MouseInput;

        private readonly CameraController CameraController;

        private readonly IList<IMenu> Menus;
        private readonly EntityWindow EntityWindow;

        private bool setCamera;

        public UIManager(Game game, SpriteBatch spriteBatch, ImGuiRenderer gui, RenderTargetDescriber renderTargetDescriber, SceneSelector sceneSelector, EntityController entityController, CameraController cameraController, Editors editors, IList<IMenu> menus, EntityWindow entityWindow, KeyboardInput keyboardInput, MouseInput mouseInput)
        {
            this.Gui = gui;
            this.KeyboardInput = keyboardInput;
            this.MouseInput = mouseInput;
            this.GameLoop = game;
            this.SpriteBatch = spriteBatch;
            this.CameraController = cameraController;
            this.RenderTargetDescriber = renderTargetDescriber;

            this.Menus = menus;
            this.EntityWindow = entityWindow;
            this.Editors = editors;

            this.State = UIState.Deserialize();

            if (!string.IsNullOrEmpty(this.State.EditorState.Scene))
            {
                var scene = sceneSelector.Scenes.FirstOrDefault(s => s.Name.Equals(this.State.EditorState.Scene, System.StringComparison.OrdinalIgnoreCase));
                if (scene != null && sceneSelector.CurrentScene != scene)
                {
                    sceneSelector.SwitchScenes(scene);
                }
            }

            if (sceneSelector.CurrentScene == null)
            {
                sceneSelector.SwitchScenes(sceneSelector.Scenes.First());
            }

            if (this.State.EntityState.SelectedEntity.Id > 0)
            {
                this.State.EntityState.SelectedEntity = entityController.GetAllEntities().FirstOrDefault(e => e.Id == this.State.EntityState.SelectedEntity.Id);
            }

            this.setCamera = true;

            for (var i = 0; i < this.Menus.Count; i++)
            {
                this.Menus[i].State = this.State;
            }
            this.EntityWindow.State = this.State;
        }

        public void Render(IScene currentScene, PerspectiveCamera camera, Viewport viewport, GameTime gameTime)
        {
            if (this.setCamera)
            {
                camera.Move(this.State.EditorState.CameraPosition, this.State.EditorState.CameraLookAt);
                this.CameraController.TranslateSpeed = this.State.EditorState.CameraSpeed;
                this.setCamera = false;
            }

            // Do not handle input if game window is not activated
            if (!this.GameLoop.IsActive)
            {
                return;
            }

            var elapsed = (Seconds)gameTime.ElapsedGameTime;

            this.KeyboardInput.Update();
            this.MouseInput.Update();

            this.CameraController.Update(camera, elapsed);

            if (this.KeyboardInput.Click(Keys.F12))
            {
                this.State.EditorState.ShowGui = !this.State.EditorState.ShowGui;
            }

            if (this.KeyboardInput.Click(Keys.Escape))
            {
                this.GameLoop.Exit();
            }

            this.RenderOverlay(viewport);
            this.RenderUI(currentScene, camera, gameTime);
        }

        private void RenderOverlay(Viewport viewport)
        {
            this.SpriteBatch.Begin(
              SpriteSortMode.Deferred,
              BlendState.Opaque,
              SamplerState.LinearClamp,
              DepthStencilState.None,
              RasterizerState.CullCounterClockwise);

            switch (this.State.DebugState.DebugDisplay)
            {
                case DebugDisplay.None:
                    break;
                case DebugDisplay.Single:
                    this.SpriteBatch.Draw(
                        this.RenderTargetDescriber.GetRenderTarget(this.State.DebugState.SelectedRenderTarget),
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
                    if (this.State.DebugState.SelectedRenderTargets.Any())
                    {
                        var xStep = viewport.Width / this.State.DebugState.Columns;
                        var yStep = xStep / viewport.AspectRatio;

                        var renderTargets = this.RenderTargetDescriber.GetRenderTargets(this.State.DebugState.SelectedRenderTargets);
                        for (var i = 0; i < renderTargets.Count; i++)
                        {
                            this.SpriteBatch.Draw(
                                renderTargets[i],
                                new Vector2(xStep * (i % this.State.DebugState.Columns), yStep * (i / this.State.DebugState.Columns)),
                                null,
                                Color.White,
                                0.0f,
                                Vector2.Zero,
                                1.0f / this.State.DebugState.Columns,
                                SpriteEffects.None,
                                0);
                        }
                    }
                    break;
            }

            this.SpriteBatch.End();
        }

        private void RenderUI(IScene currentScene, PerspectiveCamera camera, GameTime gameTime)
        {
            if (this.State.EditorState.ShowGui)
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
                        for (var i = 0; i < this.Menus.Count; i++)
                        {
                            this.Menus[i].Render(camera);
                        }

                        currentScene.RenderUI();

                        var speed = this.CameraController.TranslateSpeed.Value;
                        ImGui.SliderFloat("Movement Speed", ref speed, CameraController.MinTranslateSpeed, CameraController.MaxTranslateSpeed);
                        this.CameraController.TranslateSpeed = speed;
                        this.State.EditorState.CameraSpeed = speed;

                        ImGui.EndMainMenuBar();
                    }

                    if (this.State.EntityState.ShowEntityWindow && this.State.EntityState.SelectedEntity.Id > 0)
                    {
                        this.EntityWindow.Render();
                    }
                    else
                    {
                        this.State.EntityState.ShowEntityWindow = false;
                    }

                    if (this.State.DebugState.ShowDemo) { ImGui.ShowDemoWindow(); }
                }
                this.Gui.EndLayout();
            }
        }

        public void Close(IViewPoint viewPoint)
            => this.State.Serialize(viewPoint.Position, viewPoint.Position + viewPoint.Forward);
    }
}
