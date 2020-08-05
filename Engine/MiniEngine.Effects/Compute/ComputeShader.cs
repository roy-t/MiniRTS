using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;

namespace MiniEngine.Effects.Compute
{
    public sealed class ComputeShader : System.IDisposable
    {
        private readonly Device Device;
        private readonly DeviceContext Context;
        private readonly SharpDX.Direct3D11.ComputeShader Shader;

        private readonly Dictionary<string, ResourceBinding> Resources;

        public ComputeShader(GraphicsDevice device, string fileName, string kernel)
        {
            this.Device = device.Handle as Device;
            this.Context = this.Device.ImmediateContext;

            var byteCode = ShaderBytecode.CompileFromFile(fileName, kernel, "cs_5_0");
            this.Shader = new SharpDX.Direct3D11.ComputeShader(this.Device, byteCode);

            this.Resources = new Dictionary<string, ResourceBinding>();

            using (var reflector = new ShaderReflection(byteCode))
            {
                this.ReflectInputs(reflector, fileName);
            }
        }

        public static int GetDispatchSize(int threadGroupSize, int elements)
          => (elements + threadGroupSize - 1) / threadGroupSize;

        private void ReflectInputs(ShaderReflection reflector, string filename)
        {
            for (var i = 0; i < reflector.Description.BoundResources; i++)
            {
                var description = reflector.GetResourceBindingDescription(i);
                var register = description.BindPoint;
                var name = description.Name;

                var type = description.Type switch
                {
                    ShaderInputType.ConstantBuffer => ShaderResourceType.ConstantBuffer,
                    ShaderInputType.Structured => ShaderResourceType.StructuredBuffer,
                    ShaderInputType.UnorderedAccessViewRWStructured => ShaderResourceType.RWStructuredBuffer,
                    _ => throw new System.NotSupportedException($"Shader {filename} contains unsupported resource type {description.Type}")
                };

                this.Resources.Add(name, new ResourceBinding(register, name, type));
            }
        }

        public void Compute(int dispatchX, int dispatchY, int dispatchZ)
        {
            this.Context.ComputeShader.Set(this.Shader);

            foreach (var value in this.Resources.Values)
            {
                switch (value.ResourceType)
                {
                    case ShaderResourceType.ConstantBuffer:
                        this.Context.ComputeShader.SetConstantBuffer(value.Register, value.Buffer);
                        break;
                    case ShaderResourceType.StructuredBuffer:
                        this.Context.ComputeShader.SetShaderResource(value.Register, (ShaderResourceView)value.View);
                        break;
                    case ShaderResourceType.RWStructuredBuffer:
                        this.Context.ComputeShader.SetUnorderedAccessView(value.Register, (UnorderedAccessView)value.View);
                        break;
                }
            }

            this.Context.Dispatch(dispatchX, dispatchY, dispatchZ);

            this.Context.ComputeShader.Set(null);

            foreach (var value in this.Resources.Values)
            {
                switch (value.ResourceType)
                {
                    case ShaderResourceType.ConstantBuffer:
                        this.Context.ComputeShader.SetConstantBuffer(value.Register, null);
                        break;
                    case ShaderResourceType.StructuredBuffer:
                        this.Context.ComputeShader.SetShaderResource(value.Register, null);
                        break;
                    case ShaderResourceType.RWStructuredBuffer:
                        this.Context.ComputeShader.SetUnorderedAccessView(value.Register, null);
                        break;
                }
            }
        }

        public T[] CopyDataToCPU<T>(int elements, string resourceName)
            where T : struct
        {
            var resource = this.Resources[resourceName];
            if (resource.Buffer == null)
            {
                throw new System.ArgumentException($"Buffer has not been allocated, did you forget got call ${nameof(AllocateResource)}?", resourceName);
            }

            var stagingBuffer = this.CreateStagingBuffer<T>(elements);


            this.Context.CopyResource(resource.Buffer, stagingBuffer);
            this.Context.MapSubresource(stagingBuffer, 0, MapMode.ReadWrite, MapFlags.None, out var stream);

            this.Context.Flush();

            var result = stream.ReadRange<T>(elements);
            this.Context.UnmapSubresource(stagingBuffer, 0);

            return result;
        }

        public void SetResource<T>(string name, T data)
            where T : struct => this.SetResource<T>(name, new T[] { data });

        public void SetResource<T>(string name, T[] data)
            where T : struct
        {
            var binding = this.Resources[name];

            switch (binding.ResourceType)
            {
                case ShaderResourceType.ConstantBuffer:
                    if (binding.Buffer != null)
                    {
                        this.Context.UpdateSubresource(data, binding.Buffer);
                    }
                    else
                    {
                        binding.Buffer = this.CreateConstantBuffer(data);
                    }
                    break;
                case ShaderResourceType.StructuredBuffer:
                    binding.View?.Dispose();
                    binding.Buffer?.Dispose();
                    binding.Buffer = this.CreateStructuredBuffer(data);
                    binding.View = this.CreateShaderResourceView(binding.Buffer, data.Length);
                    break;
                case ShaderResourceType.RWStructuredBuffer:
                    throw new System.NotSupportedException($"Cannot upload data to a RWStructuredBuffer, use {nameof(AllocateResource)} instead");
            }
        }

        public void AllocateResource<T>(string name, int elements)
            where T : struct
        {
            var binding = this.Resources[name];
            binding.View?.Dispose();
            binding.Buffer?.Dispose();

            switch (binding.ResourceType)
            {
                case ShaderResourceType.ConstantBuffer:
                    throw new System.NotSupportedException($"Cannot allocate a constant buffer as it requires data on creation, use {nameof(SetResource)} instead");
                case ShaderResourceType.StructuredBuffer:
                    throw new System.NotSupportedException($"Cannot allocate a structured buffer as it requires data on creation, use {nameof(SetResource)} instead");
                case ShaderResourceType.RWStructuredBuffer:
                    binding.Buffer = this.CreateRWStructuredBuffer<T>(elements);
                    binding.View = this.CreateUnorderedAccessView(binding.Buffer, elements);
                    break;
            }
        }

        // Creates a StructuredBuffer and fills it with data that can be read only by the GPU
        private Buffer CreateStructuredBuffer<T>(T[] data)
            where T : struct
        {
            var size = Utilities.SizeOf<T>();
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

        // Create a RWStructuredBuffer that the GPU and read and write to
        private Buffer CreateRWStructuredBuffer<T>(int elements)
            where T : struct
        {
            var size = Utilities.SizeOf<T>();
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

        // Create a view of a buffer so that the shader can read it
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

        // Creates a view of a buffer so that the shader can read and write to it
        private UnorderedAccessView CreateUnorderedAccessView(Buffer buffer, int elements)
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

        // Creates a constant buffer, useful for uploading a small set of settings to the GPU
        private Buffer CreateConstantBuffer<T>(T[] data)
            where T : struct
        {
            var size = Utilities.SizeOf<T>();
            if (size % 16 != 0)
            {
                throw new System.NotSupportedException($"A constant buffer must be a multiple of 16 bytes but was {size} bytes");
            }

            return Buffer.Create(this.Device, BindFlags.ConstantBuffer, data);
        }


        // Creates a staging buffer which is used to copy data from GPU to CPU
        private Buffer CreateStagingBuffer<T>(int elements)
            where T : struct
        {
            var size = Utilities.SizeOf<T>();
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

        public void Dispose()
        {
            foreach (var resource in this.Resources.Values)
            {
                resource?.Dispose();
            }

            this.Resources.Clear();

            this.Shader?.Dispose();
        }
    }
}
