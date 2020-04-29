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
        public ByteSimpleMemory CreateByteSimpleMemory(long length) => new ByteSimpleMemory(this, length);
        public CharSimpleMemory CreateCharSimpleMemory(long length) => new CharSimpleMemory(this, length);
        public ShortSimpleMemory CreateShortSimpleMemory(long length) => new ShortSimpleMemory(this, length);
        public IntSimpleMemory CreateIntSimpleMemory(long length) => new IntSimpleMemory(this, length);
        public LongSimpleMemory CreateLongSimpleMemory(long length) => new LongSimpleMemory(this, length);
        public FloatSimpleMemory CreateFloatSimpleMemory(long length) => new FloatSimpleMemory(this, length);
        public DoubleSimpleMemory CreateDoubleSimpleMemory(long length) => new DoubleSimpleMemory(this, length);



        public ByteSimpleMemory CreateSimpleMemory(byte[] data) => new ByteSimpleMemory(this, data);
        public CharSimpleMemory CreateSimpleMemory(char[] data) => new CharSimpleMemory(this, data);
        public ShortSimpleMemory CreateSimpleMemory(short[] data) => new ShortSimpleMemory(this, data);
        public IntSimpleMemory CreateSimpleMemory(int[] data) => new IntSimpleMemory(this, data);
        public LongSimpleMemory CreateSimpleMemory(long[] data) => new LongSimpleMemory(this, data);
        public FloatSimpleMemory CreateSimpleMemory(float[] data) => new FloatSimpleMemory(this, data);
        public DoubleSimpleMemory CreateSimpleMemory(double[] data) => new DoubleSimpleMemory(this, data);

        public MappingMemory CreateMappingMemory(long size) => new MappingMemory(this, size);
        public ByteMappingMemory CreateByteMappingMemory(long length) => new ByteMappingMemory(this, length);
        public CharMappingMemory CreateCharMappingMemory(long length) => new CharMappingMemory(this, length);
        public ShortMappingMemory CreateShortMappingMemory(long length) => new ShortMappingMemory(this, length);
        public IntMappingMemory CreateIntMappingMemory(long length) => new IntMappingMemory(this, length);
        public LongMappingMemory CreateLongMappingMemory(long length) => new LongMappingMemory(this, length);
        public FloatMappingMemory CreateFloatMappingMemory(long length) => new FloatMappingMemory(this, length);
        public DoubleMappingMemory CreateDoubleMappingMemory(long length) => new DoubleMappingMemory(this, length);

        public ByteMappingMemory CreateMappingMemory(byte[] data) => new ByteMappingMemory(this, data);
        public CharMappingMemory CreateMappingMemory(char[] data) => new CharMappingMemory(this, data);
        public ShortMappingMemory CreateMappingMemory(short[] data) => new ShortMappingMemory(this, data);
        public IntMappingMemory CreateMappingMemory(int[] data) => new IntMappingMemory(this, data);
        public LongMappingMemory CreateMappingMemory(long[] data) => new LongMappingMemory(this, data);
        public FloatMappingMemory CreateMappingMemory(float[] data) => new FloatMappingMemory(this, data);
        public DoubleMappingMemory CreateMappingMemory(double[] data) => new DoubleMappingMemory(this, data);

        public SVMBuffer CreateSVMBuffer(long size, uint alignment)
            => new SVMBuffer(this, size, alignment);

        public ByteSVMBuffer CreateByteSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new ByteSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);

        public CharSVMBuffer CreateCharSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new CharSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);

        public ShortSVMBuffer CreateShortSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new ShortSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);

        public IntSVMBuffer CreateIntSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new IntSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);

        public LongSVMBuffer CreateLongSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new LongSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);

        public FloatSVMBuffer CreateFloatSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new FloatSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);

        public DoubleSVMBuffer CreateDoubleSVMBuffer(int lengthX, int lengthY = 1, int lengthZ = 1, uint alignment = 0)
            => new DoubleSVMBuffer(this, lengthX, lengthY, lengthZ, alignment);


        protected void DisposeUnManaged()
        {
            OpenCL.clReleaseContext(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
