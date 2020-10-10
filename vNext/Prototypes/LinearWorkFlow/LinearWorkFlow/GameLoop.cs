using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LinearWorkFlow
{
    public class GameLoop : Game
    {
        private const SurfaceFormat surfaceFormat = SurfaceFormat.Color;
        private readonly GraphicsDeviceManager Graphics;

        private Texture2D red;
        private Texture2D green;

        private RenderTarget2D renderTargetA;
        private RenderTarget2D renderTargetB;

        private Effect effect;

        private VertexPositionTexture[] vertices;
        private short[] indices;

        private KeyboardState lastState;
        private bool useGammeCorrectness;

        public GameLoop()
        {
            this.Graphics = new GraphicsDeviceManager(this)
            {
                GraphicsProfile = GraphicsProfile.HiDef
            };

            this.Graphics.PreferredBackBufferFormat = surfaceFormat;
            this.Content.RootDirectory = "Content";
            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            this.red = new Texture2D(this.GraphicsDevice, 16, 16, false, surfaceFormat);
            this.red.SetData(Enumerable.Repeat(Color.Red, this.red.Width * this.red.Height).ToArray());

            this.green = new Texture2D(this.GraphicsDevice, 16, 16, false, surfaceFormat);
            this.green.SetData(Enumerable.Repeat(Color.Green, this.green.Width * this.green.Height).ToArray());

            this.renderTargetA = new RenderTarget2D(this.Graphics.GraphicsDevice, this.Graphics.PreferredBackBufferWidth, this.Graphics.PreferredBackBufferHeight, false, surfaceFormat, DepthFormat.None);
            this.renderTargetB = new RenderTarget2D(this.Graphics.GraphicsDevice, this.Graphics.PreferredBackBufferWidth, this.Graphics.PreferredBackBufferHeight, false, surfaceFormat, DepthFormat.None);

            this.effect = this.Content.Load<Effect>("Shader");

            this.vertices = new VertexPositionTexture[]
            {
                new VertexPositionTexture(new Vector3(-0.5f, 0.5f, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(0.5f, 0.5f, 0), new Vector2(1, 0)),
                new VertexPositionTexture(new Vector3(0.5f, -0.5f, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(-0.5f, -0.5f, 0), new Vector2(0, 1)),
            };

            this.indices = new short[]
            {
                0, 1, 2,
                2, 3, 0
            };
        }

        protected override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Space) && this.lastState.IsKeyUp(Keys.Space))
            {
                this.useGammeCorrectness = !this.useGammeCorrectness;
            }

            this.lastState = state;

            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            var fullScreenMatrix = Matrix.CreateScale(2.0f);

            // Draw the Red-Green test image to renderTargetA
            this.GraphicsDevice.SetRenderTarget(this.renderTargetA);
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            var redMatrix = Matrix.CreateScale(Vector3.One + Vector3.UnitY) * Matrix.CreateTranslation(Vector3.Left * 0.5f);
            this.DrawTextured(this.red, redMatrix);

            var greenMatrix = Matrix.CreateScale(Vector3.One + Vector3.UnitY) * Matrix.CreateTranslation(Vector3.Right * 0.5f);
            this.DrawTextured(this.green, greenMatrix);

            // Draw a blurred version of renderTargetA to renderTargetB
            this.GraphicsDevice.SetRenderTarget(this.renderTargetB);
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            if (this.useGammeCorrectness)
            {
                this.Window.Title = "With Gamma Correction in Shader";
                this.DrawBlurredLinear(this.renderTargetA, fullScreenMatrix);
            }
            else
            {
                this.Window.Title = "Without Gamma Correction in Shader";
                this.DrawBlurred(this.renderTargetA, fullScreenMatrix);
            }

            // Display renderTargetB on screen
            this.GraphicsDevice.SetRenderTarget(null);
            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this.DrawTextured(this.renderTargetB, fullScreenMatrix);

            base.Draw(gameTime);
        }

        private void DrawTextured(Texture2D texture, Matrix worldViewProjection)
        {
            this.effect.Parameters["Texture"].SetValue(texture);
            this.effect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);

            this.effect.Techniques["Textured"].Passes[0].Apply();
            this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length, this.indices, 0, this.indices.Length / 3);
        }

        private void DrawBlurred(Texture2D texture, Matrix worldViewProjection)
        {
            this.effect.Parameters["Texture"].SetValue(texture);
            this.effect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);

            this.effect.Techniques["Blurred"].Passes[0].Apply();
            this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length, this.indices, 0, this.indices.Length / 3);
        }

        private void DrawBlurredLinear(Texture2D texture, Matrix worldViewProjection)
        {
            this.effect.Parameters["Texture"].SetValue(texture);
            this.effect.Parameters["WorldViewProjection"].SetValue(worldViewProjection);

            this.effect.Techniques["BlurredInLinearSpace"].Passes[0].Apply();
            this.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this.vertices, 0, this.vertices.Length, this.indices, 0, this.indices.Length / 3);
        }
    }
}
