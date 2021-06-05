using System;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Function;

namespace OpenCLforNet.Memory
{
    /// <summary>
    /// OpenCL memory object
    /// </summary>
    public unsafe class SimpleMemory : AbstractMemory
    {
        protected SimpleMemory() { }
        /// <summary>
        /// clCreateBuffer(context, CL_MEM_READ_WRITE, size, null, &amp;status)
        /// </summary>
        /// <param name="context">OpenCL context</param>
        /// <param name="size">Size in bytes</param>
        public SimpleMemory(Context context, long size)
        {
            Size = size;
            Context = context;
            var status = cl_status_code.CL_SUCCESS;
            Pointer = OpenCL.clCreateBuffer(context.Pointer, cl_mem_flags.CL_MEM_READ_WRITE, new IntPtr(size), null, &status);
            status.CheckError();
        }
        ///<inheritdoc cref="CreateSimpleMemory(Context, void*, long)"/>
        ///<param name="context">OpenCL context</param>
        /// <param name="data">Data to copy to the buffer</param>
        /// <param name="size">Size of <c>data</c> in bytes</param>
        public SimpleMemory(Context context, IntPtr data, long size)
        {
            CreateSimpleMemory(context, (void*)data, size);
        }
        ///<inheritdoc cref="CreateSimpleMemory(Context, void*, long)"/>
        public SimpleMemory(Context context, void* data, long size)
        {
            CreateSimpleMemory(context, data, size);
        }
        /// <summary>
        /// clCreateBuffer(context, CL_MEM_COPY_HOST_PTR | CL_MEM_READ_WRITE, size, dataPointer, &amp;status)
        /// </summary>
        /// <param name="context">OpenCL context</param>
        /// <param name="dataPointer">Data to copy to the buffer</param>
        /// <param name="size">Size of <c>dataPointer</c> in bytes</param>
        protected void CreateSimpleMemory(Context context, void* dataPointer, long size)
        {
            Size = size;
            Context = context;

            var status = cl_status_code.CL_SUCCESS;
            Pointer = OpenCL.clCreateBuffer(context.Pointer, cl_mem_flags.CL_MEM_COPY_HOST_PTR | cl_mem_flags.CL_MEM_READ_WRITE, new IntPtr(size), dataPointer, &status);
            status.CheckError();
        }

    }
}
