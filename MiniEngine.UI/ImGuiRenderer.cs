using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MiniEngine.UI
{
    /// <summary>
    /// From: https://github.com/mellinoe/ImGui.NET
    /// </summary>
    /// <summary>
    /// ImGui renderer for use with XNA-likes (FNA & MonoGame)
    /// </summary>
    public sealed class ImGuiRenderer
    {
        private readonly Game Game;

        // Graphics
        private readonly GraphicsDevice GraphicsDevice;
        private readonly RasterizerState RasterizerState;

        private BasicEffect effect;
        
        private byte[] vertexData;
        private VertexBuffer vertexBuffer;
        private int vertexBufferSize;

        private byte[] indexData;
        private IndexBuffer indexBuffer;
        private int indexBufferSize;

        // Textures
        private readonly Dictionary<IntPtr, Texture2D> LoadedTextures;

        private int textureId;
        private IntPtr? fontTextureId;

        // Input
        private int scrollWheelValue;

        private List<int> keys = new List<int>();

        public ImGuiRenderer(Game game)
        {
            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

            this.Game = game ?? throw new ArgumentNullException(nameof(game));
            this.GraphicsDevice = game.GraphicsDevice;

            this.LoadedTextures = new Dictionary<IntPtr, Texture2D>();

            this.RasterizerState = new RasterizerState()
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                FillMode = FillMode.Solid,
                MultiSampleAntiAlias = false,
                ScissorTestEnable = true,
                SlopeScaleDepthBias = 0
            };

            this.SetupInput();
        }

        /// <summary>
        /// Creates a texture and loads the font data from ImGui. Should be called when the <see cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice" /> is initialized but before any rendering is done
        /// </summary>
        public unsafe void RebuildFontAtlas()
        {
            // Get font texture from ImGui
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out int width, out int height, out int bytesPerPixel);

            // Copy the data to a managed array
            var pixels = new byte[width * height * bytesPerPixel];
            unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

            // Create and register the texture as an XNA texture
            var tex2d = new Texture2D(this.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            tex2d.SetData(pixels);

            // Should a texture already have been build previously, unbind it first so it can be deallocated
            if (this.fontTextureId.HasValue) this.UnbindTexture(this.fontTextureId.Value);

            // Bind the new texture to an ImGui-friendly id
            this.fontTextureId = this.BindTexture(tex2d);

            // Let ImGui know where to find the texture
            io.Fonts.SetTexID(this.fontTextureId.Value);
            io.Fonts.ClearTexData(); // Clears CPU side texture data
        }

        /// <summary>
        /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see cref="ImGui.Image" />. That pointer is then used by ImGui to let us know what texture to draw
        /// </summary>
        public IntPtr BindTexture(Texture2D texture)
        {
            var id = new IntPtr(this.textureId++);

            this.LoadedTextures.Add(id, texture);

            return id;
        }

        /// <summary>
        /// Removes a previously created texture pointer, releasing its reference and allowing it to be deallocated
        /// </summary>
        public void UnbindTexture(IntPtr textureId)
        {
            this.LoadedTextures.Remove(textureId);
        }

        /// <summary>
        /// Sets up ImGui for a new frame, should be called at frame start
        /// </summary>
        public void BeginLayout(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.UpdateInput();

            ImGui.NewFrame();
        }

        /// <summary>
        /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should be called after the UI is drawn using ImGui.** calls
        /// </summary>
        public void EndLayout()
        {
            ImGui.Render();

            unsafe { this.RenderDrawData(ImGui.GetDrawData()); }
        }

        /// <summary>
        /// Maps ImGui keys to XNA keys. We use this later on to tell ImGui what keys were pressed
        /// </summary>
        private void SetupInput()
        {
            var io = ImGui.GetIO();

            this.keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Back);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Keys.A);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Keys.C);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Keys.V);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Keys.X);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Keys.Y);
            this.keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Keys.Z);
         
            // MonoGame-specific //////////////////////
            this.Game.Window.TextInput += (s, a) =>
            {
                if (a.Character == '\t') return;

                io.AddInputCharacter(a.Character);
            };
            ///////////////////////////////////////////

            //ImGui.GetIO().Fonts.AddFontDefault();
            io.Fonts.AddFontFromFileTTF("Roboto-Regular.ttf", 18);
        }

        /// <summary>
        /// Updates the <see cref="Microsoft.Xna.Framework.Graphics.Effect" /> to the current matrices and texture
        /// </summary>
        private Effect UpdateEffect(Texture2D texture)
        {
            this.effect = this.effect ?? new BasicEffect(this.GraphicsDevice);

            var io = ImGui.GetIO();

            // MonoGame-specific //////////////////////
            var offset = 0.0f; // -> Might be 0.5f for the OpenGL version? See: https://github.com/mellinoe/ImGui.NET/issues/97
            ///////////////////////////////////////////

            this.effect.World = Matrix.Identity;
            this.effect.View = Matrix.Identity;
            this.effect.Projection = Matrix.CreateOrthographicOffCenter(offset, io.DisplaySize.X + offset, io.DisplaySize.Y + offset, offset, -1f, 1f);
            this.effect.TextureEnabled = true;
            this.effect.Texture = texture;
            this.effect.VertexColorEnabled = true;

            return this.effect;
        }

        /// <summary>
        /// Sends XNA input state to ImGui
        /// </summary>
        private void UpdateInput()
        {
            var io = ImGui.GetIO();

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            for (int i = 0; i < this.keys.Count; i++)
            {
                io.KeysDown[this.keys[i]] = keyboard.IsKeyDown((Keys)this.keys[i]);
            }

            io.KeyShift = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
            io.KeyCtrl = keyboard.IsKeyDown(Keys.LeftControl) || keyboard.IsKeyDown(Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Keys.LeftAlt) || keyboard.IsKeyDown(Keys.RightAlt);
            io.KeySuper = keyboard.IsKeyDown(Keys.LeftWindows) || keyboard.IsKeyDown(Keys.RightWindows);

            io.DisplaySize = new System.Numerics.Vector2(this.GraphicsDevice.PresentationParameters.BackBufferWidth, this.GraphicsDevice.PresentationParameters.BackBufferHeight);
            io.DisplayFramebufferScale = new System.Numerics.Vector2(1f, 1f);

            io.MousePos = new System.Numerics.Vector2(mouse.X, mouse.Y);

            io.MouseDown[0] = mouse.LeftButton == ButtonState.Pressed;
            io.MouseDown[1] = mouse.RightButton == ButtonState.Pressed;
            io.MouseDown[2] = mouse.MiddleButton == ButtonState.Pressed;

            var scrollDelta = mouse.ScrollWheelValue - this.scrollWheelValue;
            io.MouseWheel = scrollDelta > 0 ? 1 : scrollDelta < 0 ? -1 : 0;
            this.scrollWheelValue = mouse.ScrollWheelValue;
        }

        /// <summary>
        /// Gets the geometry as set up by ImGui and sends it to the graphics device
        /// </summary>
        private void RenderDrawData(ImDrawDataPtr drawData)
        {
            // Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, vertex/texcoord/color pointers
            var lastViewport = this.GraphicsDevice.Viewport;
            var lastScissorBox = this.GraphicsDevice.ScissorRectangle;

            this.GraphicsDevice.BlendFactor = Color.White;
            this.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            this.GraphicsDevice.RasterizerState = this.RasterizerState;
            this.GraphicsDevice.DepthStencilState = DepthStencilState.None;

            // Handle cases of screen coordinates != from framebuffer coordinates (e.g. retina displays)
            drawData.ScaleClipRects(ImGui.GetIO().DisplayFramebufferScale);

            // Setup projection
            this.GraphicsDevice.Viewport = new Viewport(0, 0, this.GraphicsDevice.PresentationParameters.BackBufferWidth, this.GraphicsDevice.PresentationParameters.BackBufferHeight);

            this.UpdateBuffers(drawData);

            this.RenderCommandLists(drawData);

            // Restore modified state
            this.GraphicsDevice.Viewport = lastViewport;
            this.GraphicsDevice.ScissorRectangle = lastScissorBox;
        }

        private unsafe void UpdateBuffers(ImDrawDataPtr drawData)
        {
            if (drawData.TotalVtxCount == 0)
            {
                return;
            }

            // Expand buffers if we need more room
            if (drawData.TotalVtxCount > this.vertexBufferSize)
            {
                this.vertexBuffer?.Dispose();

                this.vertexBufferSize = (int)(drawData.TotalVtxCount * 1.5f);
                this.vertexBuffer = new VertexBuffer(this.GraphicsDevice, DrawVertDeclaration.Declaration, this.vertexBufferSize, BufferUsage.None);
                this.vertexData = new byte[this.vertexBufferSize * DrawVertDeclaration.Size];
            }

            if (drawData.TotalIdxCount > this.indexBufferSize)
            {
                this.indexBuffer?.Dispose();

                this.indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                this.indexBuffer = new IndexBuffer(this.GraphicsDevice, IndexElementSize.SixteenBits, this.indexBufferSize, BufferUsage.None);
                this.indexData = new byte[this.indexBufferSize * sizeof(ushort)];
            }

            // Copy ImGui's vertices and indices to a set of managed byte arrays
            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &this.vertexData[vtxOffset * DrawVertDeclaration.Size])
                fixed (void* idxDstPtr = &this.indexData[idxOffset * sizeof(ushort)])
                {
                    Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, this.vertexData.Length, cmdList.VtxBuffer.Size * DrawVertDeclaration.Size);
                    Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, this.indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers
            this.vertexBuffer.SetData(this.vertexData, 0, drawData.TotalVtxCount * DrawVertDeclaration.Size);
            this.indexBuffer.SetData(this.indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private void RenderCommandLists(ImDrawDataPtr drawData)
        {
            this.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            this.GraphicsDevice.Indices = this.indexBuffer;

            int vtxOffset = 0;
            int idxOffset = 0;

            for (int n = 0; n < drawData.CmdListsCount; n++)
            {
                ImDrawListPtr cmdList = drawData.CmdListsRange[n];

                for (int cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    ImDrawCmdPtr drawCmd = cmdList.CmdBuffer[cmdi];

                    if (!this.LoadedTextures.ContainsKey(drawCmd.TextureId))
                    {
                        throw new InvalidOperationException($"Could not find a texture with id '{drawCmd.TextureId}', please check your bindings");
                    }

                    this.GraphicsDevice.ScissorRectangle = new Rectangle(
                        (int)drawCmd.ClipRect.X,
                        (int)drawCmd.ClipRect.Y,
                        (int)(drawCmd.ClipRect.Z - drawCmd.ClipRect.X),
                        (int)(drawCmd.ClipRect.W - drawCmd.ClipRect.Y)
                    );

                    var effect = this.UpdateEffect(this.LoadedTextures[drawCmd.TextureId]);

                    foreach (var pass in effect.CurrentTechnique.Passes)
                    {
                        pass.Apply();
                        this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, vtxOffset, idxOffset, (int)drawCmd.ElemCount / 3);
                    }

                    idxOffset += (int)drawCmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
        }
    }
}
