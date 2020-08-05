using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Compute;
using MiniEngine.Pipeline.Models.Components;
using MiniEngine.Primitives.VertexTypes;

namespace MiniEngine.Pipeline.Models.Generators
{
    public sealed class NoiseGenerator
    {
        private readonly GraphicsDevice Device;
        private readonly ContentManager Content;

        public NoiseGenerator(GraphicsDevice device, ContentManager content)
        {
            this.Device = device;
            this.Content = content;
        }

        public void GenerateNoise(Geometry inputGeometry)
        {
            try
            {
                var file = Path.GetFullPath(Path.Join(this.Content.RootDirectory, @"ComputeShaders\Noise.hlsl"));
                var shader = new ComputeShader(this.Device, file, "Kernel");

                shader.SetResource("Settings", new Settings { multiplier = 2 });
                shader.SetResource("InputGeometry", inputGeometry.Vertices);
                shader.AllocateResource<GBufferVertex>("OutputGeometry", inputGeometry.VertexCount);


                var dispatchSize = ComputeShader.GetDispatchSize(256, inputGeometry.VertexCount);
                shader.Compute(dispatchSize, 1, 1);

                var data = shader.CopyDataToCPU<GBufferVertex>(inputGeometry.VertexCount, "OutputGeometry");
                Array.Copy(data, inputGeometry.Vertices, data.Length);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Settings
        {
            public int multiplier;
            public int _padding0;
            public int _padding1;
            public int _padding2;
        }
    }
}
