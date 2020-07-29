using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Compute;

namespace MiniEngine.Pipeline.Models.Generators
{
    public sealed class NoiseGenerator
    {
        private const int Repetition = (64 * 32) * (64 * 32);
        private readonly GraphicsDevice Device;
        private readonly ContentManager Content;

        public NoiseGenerator(GraphicsDevice device, ContentManager content)
        {
            this.Device = device;
            this.Content = content;
        }

        public void GenerateNoise()
        {
            try
            {
                var file = Path.GetFullPath(Path.Join(this.Content.RootDirectory, @"ComputeShaders\maclaurin.hlsl"));

                var shader = new ComputeShader<Vector4>(this.Device, file, "CS2", Repetition);
                var data = shader.Compute(64, 64, 1, Repetition);
                Debug.WriteLine(data[0].ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private struct ResultData
        {
            public float functionResult;
            public int x;
            public float unused;
            public float unused2;

            public override string ToString()
            {
                return string.Format("X: {0} Y: {1}", x, functionResult);
            }
        }
    }
}
