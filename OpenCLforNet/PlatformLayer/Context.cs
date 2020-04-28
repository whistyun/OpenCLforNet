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

        public SimpleMemory CreateSimpleMemory(long size)
        {
            return new SimpleMemory(this, size);
        }

        public SimpleMemory CreateSimpleMemory(byte[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public SimpleMemory CreateSimpleMemory(char[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public SimpleMemory CreateSimpleMemory(short[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public SimpleMemory CreateSimpleMemory(int[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public SimpleMemory CreateSimpleMemory(long[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public SimpleMemory CreateSimpleMemory(float[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public SimpleMemory CreateSimpleMemory(double[] data, long size)
        {
            return new SimpleMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(long size)
        {
            return new MappingMemory(this, size);
        }

        public MappingMemory CreateMappingMemory(byte[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(char[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(short[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(int[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(long[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(float[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public MappingMemory CreateMappingMemory(double[] data, long size)
        {
            return new MappingMemory(this, data, size);
        }

        public SVMBuffer CreateSVMBuffer(long size, uint alignment)
        {
            return new SVMBuffer(this, size, alignment);

        }

        public ByteSVMBuffer CreateByteSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new ByteSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        public CharSVMBuffer CreateCharSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new CharSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        public ShortSVMBuffer CreateShortSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new ShortSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        public IntSVMBuffer CreateIntSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new IntSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        public LongSVMBuffer CreateLongSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new LongSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        public FloatSVMBuffer CreateFloatSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new FloatSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        public DoubleSVMBuffer CreateDoubleSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
        {
            return new DoubleSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);
        }

        protected void DisposeUnManaged()
        {
            OpenCL.clReleaseContext(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
