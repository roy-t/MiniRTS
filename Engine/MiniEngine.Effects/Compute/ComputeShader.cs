using Microsoft.Xna.Framework.Graphics;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace MiniEngine.Effects.Compute
{
    public sealed class ComputeShader<T> where T : struct
    {
        private Device Device;
        private DeviceContext Context;
        private UnorderedAccessView UnorderedAccessView;
        private ComputeShader Shader;

        private Buffer Buffer;
        private Buffer ResultBuffer;

        public ComputeShader(GraphicsDevice device, string fileName, string kernel, int elements)
        {
            this.Device = device.Handle as Device;
            this.Context = this.Device.ImmediateContext;

            this.Buffer = this.CreateUAVBuffer(elements);
            this.UnorderedAccessView = this.CreateUAV(this.Buffer, elements);

            var byteCode = ShaderBytecode.CompileFromFile(fileName, kernel, "cs_5_0");
            this.Shader = new ComputeShader(this.Device, byteCode);

            this.ResultBuffer = this.CreateStagingBuffer(elements);
        }

        public T[] Compute(int threadsX, int threadsY, int threadsZ, int elements)
        {
            this.Context.ComputeShader.SetUnorderedAccessView(0, this.UnorderedAccessView);
            this.Context.ComputeShader.Set(this.Shader);

            this.Context.Dispatch(threadsX, threadsY, threadsZ);

            this.Context.CopyResource(this.Buffer, this.ResultBuffer);
            this.Context.Flush();
            this.Context.ComputeShader.SetUnorderedAccessView(0, null);
            this.Context.ComputeShader.Set(null);

            this.Context.MapSubresource(this.ResultBuffer, 0, MapMode.Read, MapFlags.None, out var stream);
            var result = stream.ReadRange<T>(elements);
            this.Context.UnmapSubresource(this.ResultBuffer, 0);

            return result;

        }

        private Buffer CreateUAVBuffer(int elements)
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
