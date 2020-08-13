using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
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

        public void GenerateNoise(Geometry inputGeometry, NoiseSettings noiseSettings, Crater[] craters)
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var file = Path.GetFullPath(Path.Join(this.Content.RootDirectory, @"ComputeShaders\Noise.hlsl"));
                var shader = new ComputeShader(this.Device, file, "Kernel");

                shader.SetResource("Settings", noiseSettings);
                shader.SetResource("InputGeometry", inputGeometry.Vertices);
                shader.SetResource("InputCraters", craters);
                shader.AllocateResource<GBufferVertex>("OutputGeometry", inputGeometry.VertexCount);

                var dispatchSize = ComputeShader.GetDispatchSize(512, inputGeometry.VertexCount);
                shader.Compute(dispatchSize, 1, 1);

                var data = shader.CopyDataToCPU<GBufferVertex>(inputGeometry.VertexCount, "OutputGeometry");
                Array.Copy(data, inputGeometry.Vertices, data.Length);
                var time = stopwatch.ElapsedMilliseconds;
                Debug.WriteLine($"Compute shader 'Noise.hlsl' processed {inputGeometry.VertexCount} in {time}ms");

                // TODO: scale back to fit inside bounding box doesn't work yet, result looks correct after multiple runs?                
                MeshHelper.ScaleToUnitSphere(inputGeometry.Vertices); //--> 
                MeshHelper.ComputeNormals(inputGeometry.Vertices, inputGeometry.Indices);
                // TODO: also compute binormals and tangets, MonoGame\MonoGame.Framework.Content.Pipeline\Graphics\MeshHelper.cs for an example.
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NoiseSettings
    {
        public float rimWidth;
        public float rimSteepness;
        public int craterCount;
        public float padding;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Crater
    {
        public Vector3 position;
        public float radius;
        public float floor;
        public float smoothness;
    }
}
