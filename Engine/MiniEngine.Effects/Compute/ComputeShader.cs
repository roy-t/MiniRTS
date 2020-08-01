using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace MiniEngine.Effects.Compute
{
    public sealed class ComputeShader<T> where T : struct
    {
        private Device Device;
        private DeviceContext Context;
        private ShaderResourceView InputView;
        private UnorderedAccessView OutputView;
        private ComputeShader Shader;
        private ShaderReflection Reflection;

        private Buffer InputBuffer;
        private Buffer OutputBuffer;
        private Buffer StagingBuffer;

        public ComputeShader(GraphicsDevice device, string fileName, string kernel, T[] data)
        {
            var elements = data.Length;

            this.Device = device.Handle as Device;
            this.Context = this.Device.ImmediateContext;


            //this.InputBuffer = this.CreateInputBuffer(data);
            //this.InputView = this.CreateShaderResourceView(this.InputBuffer, elements);

            this.OutputBuffer = this.CreateOutputBuffer(elements);
            this.OutputView = this.CreateUAV(this.OutputBuffer, elements);

            var byteCode = ShaderBytecode.CompileFromFile(fileName, kernel, "cs_5_0");
            this.Shader = new ComputeShader(this.Device, byteCode);
            this.Reflection = new ShaderReflection(byteCode);

            this.StagingBuffer = this.CreateStagingBuffer(elements);

            // TODO: use this code to map names to registers
            for (var i = 0; i < this.Reflection.Description.BoundResources; i++)
            {
                var cb = this.Reflection.GetResourceBindingDescription(i);
                Debug.WriteLine($"{cb.Type} {cb.Name} : register({cb.BindPoint})");
            }
        }

        public static int GetDispatchSize(int threadGroupSizeX, int elements)
            => (elements + threadGroupSizeX - 1) / threadGroupSizeX;

        public T[] Compute(int threadsX, int threadsY, int threadsZ, int elements)
        {
            // Upload data
            this.Context.ComputeShader.Set(this.Shader);
            //this.Context.ComputeShader.SetShaderResource(0, this.InputView);
            this.Context.ComputeShader.SetUnorderedAccessView(0, this.OutputView);

            // Compute
            this.Context.Dispatch(threadsX, threadsY, threadsZ);

            // Unset buffers
            this.Context.ComputeShader.Set(null);
            this.Context.ComputeShader.SetUnorderedAccessView(0, null);
            //this.Context.ComputeShader.SetShaderResource(0, null);

            this.Context.MapSubresource(this.StagingBuffer, 0, MapMode.ReadWrite, MapFlags.None, out var stream);
            this.Context.CopyResource(this.OutputBuffer, this.StagingBuffer);
            this.Context.Flush();

            var result = stream.ReadRange<T>(elements);
            this.Context.UnmapSubresource(this.StagingBuffer, 0);

            return result;
        }

        private Buffer CreateInputBuffer(T[] data)
        {
            var size = SharpDX.Utilities.SizeOf<T>();
            var description = new BufferDescription()
            {
                BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.BufferStructured,
                StructureByteStride = size,
                SizeInBytes = size * data.Length
            };

            var stream = DataStream.Create(data, true, true);

            return new Buffer(this.Device, stream, description);
        }

        private Buffer CreateOutputBuffer(int elements)
        {
            var size = SharpDX.Utilities.SizeOf<T>();
            var description = new BufferDescription()
            {
                BindFlags = BindFlags.UnorderedAccess | BindFlags.ShaderResource,
                Usage = ResourceUsage.Default,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.BufferStructured,
                StructureByteStride = size,
                SizeInBytes = size * elements
            };

            return new Buffer(this.Device, description);
        }

        private UnorderedAccessView CreateUAV(Buffer buffer, int elements)
        {
            var description = new UnorderedAccessViewDescription()
            {
                Buffer = new UnorderedAccessViewDescription.BufferResource()
                {
                    FirstElement = 0,
                    Flags = UnorderedAccessViewBufferFlags.None,
                    ElementCount = elements
                },
                Format = SharpDX.DXGI.Format.Unknown,
                Dimension = UnorderedAccessViewDimension.Buffer
            };

            return new UnorderedAccessView(this.Device, buffer, description);
        }

        private ShaderResourceView CreateShaderResourceView(Buffer buffer, int elements)
        {
            var description = new ShaderResourceViewDescription()
            {
                Buffer = new ShaderResourceViewDescription.BufferResource()
                {
                    FirstElement = 0,
                    ElementCount = elements
                },
                Format = SharpDX.DXGI.Format.Unknown,
                Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Buffer
            };

            return new ShaderResourceView(this.Device, buffer, description);
        }

        private Buffer CreateStagingBuffer(int elements)
        {
            var size = SharpDX.Utilities.SizeOf<T>();
            var description = new BufferDescription()
            {
                BindFlags = BindFlags.None,
                Usage = ResourceUsage.Staging,
                CpuAccessFlags = CpuAccessFlags.Read | CpuAccessFlags.Write,
                OptionFlags = ResourceOptionFlags.None,
                StructureByteStride = size,
                SizeInBytes = size * elements
            };

            return new Buffer(this.Device, description);
        }
    }
}
