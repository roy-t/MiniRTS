using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MiniEngine.Configuration;
using MiniEngine.Graphics.Generated;

namespace MiniEngine.Gui
{
    [Service]
    public sealed class ImGuiRenderer : IDisposable
    {
        // Graphics
        private readonly GraphicsDevice GraphicsDevice;

        private readonly Game Game;

        private readonly ImmediateEffect Effect;
        private readonly RasterizerState RasterizerState;

        private byte[]? vertexData;
        private VertexBuffer? vertexBuffer;
        private int vertexBufferSize;

        private byte[]? indexData;
        private IndexBuffer? indexBuffer;
        private int indexBufferSize;

        // Textures
        private readonly Dictionary<IntPtr, Texture2D> LoadedTextures;

        private int textureId;
        private IntPtr? fontTextureId;

        // Input
        private int scrollWheelValue;

        private readonly List<int> Keys = new List<int>();

        public ImGuiRenderer(GraphicsDevice device, Game game, ImmediateEffect effect)
        {
            this.GraphicsDevice = device;
            this.Game = game;
            this.Effect = effect;

            var context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);

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

            var io = ImGui.GetIO();
            io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

            this.SetupInput();
            this.RebuildFontAtlas();
        }

        /// <summary>
        /// Creates a texture and loads the font data from ImGui. Should be called when the <see
        /// cref="Microsoft.Xna.Framework.Graphics.GraphicsDevice"/> is initialized but before any
        /// rendering is done
        /// </summary>
        private unsafe void RebuildFontAtlas()
        {
            // Get font texture from ImGui
            var io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out byte* pixelData, out var width, out var height, out var bytesPerPixel);

            // Copy the data to a managed array
            var pixels = new byte[width * height * bytesPerPixel];
            unsafe { Marshal.Copy(new IntPtr(pixelData), pixels, 0, pixels.Length); }

            // Create and register the texture as an XNA texture
            var tex2d = new Texture2D(this.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            tex2d.SetData(pixels);

            // Should a texture already have been build previously, unbind it first so it can be deallocated
            if (this.fontTextureId.HasValue)
            {
                this.UnbindTexture(this.fontTextureId.Value);
            }

            // Bind the new texture to an ImGui-friendly id
            this.BindTexture(tex2d);
            this.fontTextureId = (IntPtr)tex2d.Tag;

            // Let ImGui know where to find the texture
            io.Fonts.SetTexID(this.fontTextureId.Value);
            io.Fonts.ClearTexData(); // Clears CPU side texture data
        }

        /// <summary>
        /// Creates a pointer to a texture, which can be passed through ImGui calls such as <see
        /// cref="ImGui.Image"/>. That pointer is then used by ImGui to let us know what texture to draw
        /// </summary>
        public void BindTexture(Texture2D texture)
        {
            if (texture.Tag == null)
            {
                var id = new IntPtr(this.textureId++);
                texture.Tag = id;
                this.LoadedTextures.Add(id, texture);
            }
        }

        /// <summary>
        /// Removes a previously created texture pointer, releasing its reference and allowing it to
        /// be deallocated
        /// </summary>
        public void UnbindTexture(IntPtr textureId) => this.LoadedTextures.Remove(textureId);

        /// <summary>
        /// Sets up ImGui for a new frame, should be called at frame start
        /// </summary>
        public void BeforeLayout(GameTime gameTime)
        {
            ImGui.GetIO().DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.Game.IsActive)
            {
                this.UpdateInput();
            }

            ImGui.NewFrame();
        }

        /// <summary>
        /// Asks ImGui for the generated geometry data and sends it to the graphics pipeline, should
        /// be called after the UI is drawn using ImGui.** calls
        /// </summary>
        public void AfterLayout()
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

            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Tab] = (int)Microsoft.Xna.Framework.Input.Keys.Tab);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Microsoft.Xna.Framework.Input.Keys.Left);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Microsoft.Xna.Framework.Input.Keys.Right);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Microsoft.Xna.Framework.Input.Keys.Up);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Microsoft.Xna.Framework.Input.Keys.Down);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.PageUp] = (int)Microsoft.Xna.Framework.Input.Keys.PageUp);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.PageDown] = (int)Microsoft.Xna.Framework.Input.Keys.PageDown);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Home] = (int)Microsoft.Xna.Framework.Input.Keys.Home);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.End] = (int)Microsoft.Xna.Framework.Input.Keys.End);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Delete] = (int)Microsoft.Xna.Framework.Input.Keys.Delete);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Backspace] = (int)Microsoft.Xna.Framework.Input.Keys.Back);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Enter] = (int)Microsoft.Xna.Framework.Input.Keys.Enter);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Escape] = (int)Microsoft.Xna.Framework.Input.Keys.Escape);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.A] = (int)Microsoft.Xna.Framework.Input.Keys.A);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.C] = (int)Microsoft.Xna.Framework.Input.Keys.C);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.V] = (int)Microsoft.Xna.Framework.Input.Keys.V);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.X] = (int)Microsoft.Xna.Framework.Input.Keys.X);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Y] = (int)Microsoft.Xna.Framework.Input.Keys.Y);
            this.Keys.Add(io.KeyMap[(int)ImGuiKey.Z] = (int)Microsoft.Xna.Framework.Input.Keys.Z);

            this.Game.Window.TextInput += (s, a) =>
            {
                if (a.Character == '\t')
                {
                    return;
                }

                io.AddInputCharacter(a.Character);
            };

            ImGui.GetIO().Fonts.AddFontDefault();
        }

        /// <summary>
        /// Updates the <see cref="Microsoft.Xna.Framework.Graphics.Effect"/> to the current
        /// matrices and texture
        /// </summary>
        private void UpdateEffect(Texture2D texture)
        {
            var io = ImGui.GetIO();

            this.Effect.WorldViewProjection = Matrix.CreateOrthographicOffCenter(0, io.DisplaySize.X, io.DisplaySize.Y, 0, -1f, 1f);
            this.Effect.Color = texture;
            this.Effect.ConvertColorsToLinear = texture.Format != SurfaceFormat.ColorSRgb;
            this.Effect.ColorSampler = SamplerState.LinearClamp;
        }

        /// <summary>
        /// Sends XNA input state to ImGui
        /// </summary>
        private void UpdateInput()
        {
            var io = ImGui.GetIO();

            var mouse = Mouse.GetState();
            var keyboard = Keyboard.GetState();

            for (var i = 0; i < this.Keys.Count; i++)
            {
                io.KeysDown[this.Keys[i]] = keyboard.IsKeyDown((Keys)this.Keys[i]);
            }

            io.KeyShift = keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
            io.KeyCtrl = keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
            io.KeyAlt = keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftAlt) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightAlt);
            io.KeySuper = keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftWindows) || keyboard.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightWindows);

            io.DisplaySize = new Vector2(this.GraphicsDevice.PresentationParameters.BackBufferWidth, this.GraphicsDevice.PresentationParameters.BackBufferHeight);
            io.DisplayFramebufferScale = new Vector2(1f, 1f);

            io.MousePos = new Vector2(mouse.X, mouse.Y);

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
            // Setup render state: alpha-blending enabled, no face culling, no depth testing,
            // scissor enabled, vertex/texcoord/color pointers
            var lastViewport = this.GraphicsDevice.Viewport;
            var lastScissorBox = this.GraphicsDevice.ScissorRectangle;

            this.GraphicsDevice.BlendFactor = Color.White;
            this.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            this.GraphicsDevice.RasterizerState = this.RasterizerState;
            this.GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

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
                this.vertexBuffer = new VertexBuffer(this.GraphicsDevice, ImmediateVertex.Declaration, this.vertexBufferSize, BufferUsage.WriteOnly);
                this.vertexData = new byte[this.vertexBufferSize * sizeof(ImmediateVertex)];
            }

            if (drawData.TotalIdxCount > this.indexBufferSize)
            {
                this.indexBuffer?.Dispose();

                this.indexBufferSize = (int)(drawData.TotalIdxCount * 1.5f);
                this.indexBuffer = new IndexBuffer(this.GraphicsDevice, IndexElementSize.SixteenBits, this.indexBufferSize, BufferUsage.WriteOnly);
                this.indexData = new byte[this.indexBufferSize * sizeof(ushort)];
            }

            // Copy ImGui's vertices and indices to a set of managed byte arrays
            var vtxOffset = 0;
            var idxOffset = 0;

            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdListsRange[n];

                fixed (void* vtxDstPtr = &this.vertexData![vtxOffset * sizeof(ImmediateVertex)])
                fixed (void* idxDstPtr = &this.indexData![idxOffset * sizeof(ushort)])
                {
                    Buffer.MemoryCopy((void*)cmdList.VtxBuffer.Data, vtxDstPtr, this.vertexData.Length, cmdList.VtxBuffer.Size * sizeof(ImmediateVertex));
                    Buffer.MemoryCopy((void*)cmdList.IdxBuffer.Data, idxDstPtr, this.indexData.Length, cmdList.IdxBuffer.Size * sizeof(ushort));
                }

                vtxOffset += cmdList.VtxBuffer.Size;
                idxOffset += cmdList.IdxBuffer.Size;
            }

            // Copy the managed byte arrays to the gpu vertex- and index buffers
            this.vertexBuffer!.SetData(this.vertexData, 0, drawData.TotalVtxCount * sizeof(ImmediateVertex));
            this.indexBuffer!.SetData(this.indexData, 0, drawData.TotalIdxCount * sizeof(ushort));
        }

        private unsafe void RenderCommandLists(ImDrawDataPtr drawData)
        {
            this.GraphicsDevice.SetVertexBuffer(this.vertexBuffer);
            this.GraphicsDevice.Indices = this.indexBuffer;

            var vtxOffset = 0;
            var idxOffset = 0;

            for (var n = 0; n < drawData.CmdListsCount; n++)
            {
                var cmdList = drawData.CmdListsRange[n];

                for (var cmdi = 0; cmdi < cmdList.CmdBuffer.Size; cmdi++)
                {
                    var drawCmd = cmdList.CmdBuffer[cmdi];

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

                    this.UpdateEffect(this.LoadedTextures[drawCmd.TextureId]);
                    this.Effect.Apply();
#pragma warning disable CS0618
                    this.GraphicsDevice.DrawIndexedPrimitives(
                        primitiveType: PrimitiveType.TriangleList,
                        baseVertex: vtxOffset,
                        minVertexIndex: 0,
                        numVertices: cmdList.VtxBuffer.Size,
                        startIndex: idxOffset,
                        primitiveCount: (int)drawCmd.ElemCount / 3
                    );
#pragma warning restore CS0618

                    idxOffset += (int)drawCmd.ElemCount;
                }

                vtxOffset += cmdList.VtxBuffer.Size;
            }
        }

        public void Dispose()
        {
            var ptr = ImGui.GetCurrentContext();
            if (ptr != IntPtr.Zero)
            {
                ImGui.DestroyContext();
            }
        }
    }
}
