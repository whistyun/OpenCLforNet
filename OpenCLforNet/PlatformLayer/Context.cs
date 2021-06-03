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

        public CommandQueue CreateCommandQueue(int idx = 0)
            => CreateCommandQueue(Devices[idx]);

        public CommandQueue CreateCommandQueue(Device device)
        {
            return new CommandQueue(this, device);
        }

        public CLProgram CreateProgram(string source)
        {
            return new CLProgram(source, this);
        }

        public CLProgram CreateProgram(string source, string option)
        {
            return new CLProgram(source, this, option);
        }

        public Kernel CreateKernel(string source, string kernelName)
        {
            var program = new CLProgram(source, this);
            return program.CreateKernel(kernelName);
        }

        public SimpleMemory CreateSimpleMemory(long size) => new SimpleMemory(this, size);
        public TypedSimpleMemory<T> CreateSimpleMemory<T>(long length) where T : unmanaged => new TypedSimpleMemory<T>(this, length);
        public TypedSimpleMemory<T> CreateSimpleMemory<T>(T[] data) where T : unmanaged => new TypedSimpleMemory<T>(this, data);
        public TypedSimpleMemory<T> CreateSimpleMemory<T>(T[] data, long length) where T : unmanaged => new TypedSimpleMemory<T>(this, data, length);

        public MappingMemory CreateMappingMemory(long size) => new MappingMemory(this, size);
        public TypedMappingMemory<T> CreateMappingMemory<T>(long length) where T : unmanaged => new TypedMappingMemory<T>(this, length);
        public TypedMappingMemory<T> CreateMappingMemory<T>(T[] data) where T : unmanaged => new TypedMappingMemory<T>(this, data);
        public TypedMappingMemory<T> CreateMappingMemory<T>(T[] data, long length) where T : unmanaged => new TypedMappingMemory<T>(this, data, length);

        public SVMBuffer CreateSVMBuffer(long size, uint alignment)
            => new SVMBuffer(this, size, alignment);

        public TypedSVMBuffer<T> CreateSVMBuffer<T>(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0) where T : unmanaged
            => new TypedSVMBuffer<T>(this, lengthX, lengthY, lengthZ, alignment);

        protected void DisposeUnManaged()
        {
            OpenCL.clReleaseContext(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
