using System;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Function;

namespace OpenCLforNet.Memory
{
    public unsafe class SimpleMemory : AbstractMemory
    {
        protected SimpleMemory() { }

        public SimpleMemory(Context context, long size)
        {
            Size = size;
            Context = context;
            var status = cl_status_code.CL_SUCCESS;
            Pointer = OpenCL.clCreateBuffer(context.Pointer, cl_mem_flags.CL_MEM_READ_WRITE, new IntPtr(size), null, &status);
            status.CheckError();
        }

        public SimpleMemory(Context context, IntPtr data, long size)
        {
            CreateSimpleMemory(context, (void*)data, size);
        }

        public SimpleMemory(Context context, void* data, long size)
        {
            CreateSimpleMemory(context, data, size);
        }

        protected void CreateSimpleMemory(Context context, void* dataPointer, long size)
        {
            Size = size;
            Context = context;

            var status = cl_status_code.CL_SUCCESS;
            Pointer = OpenCL.clCreateBuffer(context.Pointer, (cl_mem_flags.CL_MEM_COPY_HOST_PTR | cl_mem_flags.CL_MEM_READ_WRITE), new IntPtr(size), dataPointer, &status);
            status.CheckError();
        }

    }
}
