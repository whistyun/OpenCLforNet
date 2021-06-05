using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenCLforNet.Memory;
using OpenCLforNet.Runtime;
using OpenCLforNet.Function;

namespace OpenCLforNet.PlatformLayer
{
    public unsafe class Context : IDisposable
    {
        private bool isDisposed = false;

        public Device[] Devices { get; }
        public void* Pointer { get; }

        public Context(params Device[] devices)
        {
            Devices = devices;

            var status = cl_status_code.CL_SUCCESS;
            var devicePointers = (void**)Marshal.AllocCoTaskMem(devices.Length * IntPtr.Size);
            for (var i = 0; i < devices.Length; i++)
                devicePointers[i] = devices[i].Pointer;


            Pointer = OpenCL.clCreateContext(null, (uint)devices.Length, devicePointers, null, null, &status);
            status.CheckError();
        }

        ~Context() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool includeManaged)
        {
            if (!isDisposed)
            {
                if (includeManaged)
                    DisposeManaged();

                DisposeUnManaged();
                isDisposed = true;
            }
        }

        public CommandQueue CreateCommandQueue(int idx = 0) => CreateCommandQueue(Devices[idx]);
        public CommandQueue CreateCommandQueue(int idx, cl_command_queue_properties options) => CreateCommandQueue(Devices[idx], options);
        public CommandQueue CreateCommandQueue(Device device) => new CommandQueue(this, device);
        public CommandQueue CreateCommandQueue(Device device, cl_command_queue_properties options) => new CommandQueue(this, device, options);

        public CLProgram CreateProgram(string source) => new CLProgram(source, this);
        public CLProgram CreateProgram(string source, string option) => new CLProgram(source, this, option);

        public Kernel CreateKernel(string source, string kernelName)
        {
            var program = new CLProgram(source, this);
            return program.CreateKernel(kernelName);
        }

        public SimpleMemory CreateSimpleMemory(long size) => new SimpleMemory(this, size);
        public SimpleMemory<T> CreateSimpleMemory<T>(long length) where T : unmanaged => new SimpleMemory<T>(this, length);
        public SimpleMemory<T> CreateSimpleMemory<T>(T[] data) where T : unmanaged => new SimpleMemory<T>(this, data);
        public SimpleMemory<T> CreateSimpleMemory<T>(T[] data, long length) where T : unmanaged => new SimpleMemory<T>(this, data, length);

        public MappingMemory CreateMappingMemory(long size) => new MappingMemory(this, size);
        public MappingMemory<T> CreateMappingMemory<T>(long length) where T : unmanaged => new MappingMemory<T>(this, length);
        public MappingMemory<T> CreateMappingMemory<T>(T[] data) where T : unmanaged => new MappingMemory<T>(this, data);
        public MappingMemory<T> CreateMappingMemory<T>(T[] data, long length) where T : unmanaged => new MappingMemory<T>(this, data, length);

        public SVMBuffer CreateSVMBuffer(long size, uint alignment)
            => new SVMBuffer(this, size, alignment);

        public SVMBuffer<T> CreateSVMBuffer<T>(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0) where T : unmanaged
            => new SVMBuffer<T>(this, lengthX, lengthY, lengthZ, alignment);

        protected void DisposeUnManaged()
        {
            OpenCL.clReleaseContext(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
