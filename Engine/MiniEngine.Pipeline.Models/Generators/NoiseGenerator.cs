using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MiniEngine.Effects.Compute;

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

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Settings
        {
            public int multiplier;
            public int _padding0;
            public int _padding1;
            public int _padding2;
        }

        public void GenerateNoise()
        {
            try
            {
                // TODO: I should now be able to change the shader to set an arbitrary number of input buffers
                // and textures based on the register. Best to only use one output buffer(?) Rewrite the ComputeShader class for this.
                // Let's try to load all the vertices and move them around a bit based on a texture lookup. Looks like all standard
                // shader functions are still available. Maybe first even try a sine wave?
                // TODO: how can we set a couple of variables? -> https://forum.unity.com/threads/global-shader-variables-in-compute-shaders.471211/
                // apparently not but you can set a buffer with your variables.
                var file = Path.GetFullPath(Path.Join(this.Content.RootDirectory, @"ComputeShaders\maclaurin.hlsl"));

                var input = Enumerable.Range(0, 1000).ToArray();
                //var shader = new ComputeShader<int>(this.Device, file, "Kernel", input);
                var shader2 = new ComputeShader2(this.Device, file, "Kernel");

                //shader2.SetResource("settings", new Settings[] { new Settings { multiplier = 2 } });
                //shader2.SetResource("InputBuffer", input);
                shader2.AllocateResource<int>("OutputBuffer", 1000);

                var dispatchers = ComputeShader2.GetDispatchSize(256, 1000);
                shader2.Compute(dispatchers, 1, 1);

                var data = shader2.CopyDataToCPU<int>(1000, "OutputBuffer");

                //var dispatches = ComputeShader<int>.GetDispatchSize(256, input.Length);



                var shader = new ComputeShader<int>(this.Device, file, "Kernel", input);
                var data2 = shader.Compute(dispatchers, 1, 1, 1000);


                //var data = shader.Compute(dispatches, 1, 1, 1000);

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
