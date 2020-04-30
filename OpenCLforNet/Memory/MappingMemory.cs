using System;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using OpenCLforNet.Function;

namespace OpenCLforNet.Memory
{
    public unsafe class MappingMemory : AbstractMemory
    {
        protected MappingMemory() { }

        public MappingMemory(Context context, long size)
        {
            CreateMappingMemory(context, size);
        }
        public MappingMemory(Context context, IntPtr data, long size)
        {
            CreateMappingMemory(context, (void*)data, size);
        }
        public MappingMemory(Context context, void* data, long size)
        {
            CreateMappingMemory(context, data, size);
        }

        protected void CreateMappingMemory(Context context, void* dataPointer, long size)
        {
            var memFlg = cl_mem_flags.CL_MEM_USE_HOST_PTR | cl_mem_flags.CL_MEM_READ_WRITE;

            cl_status_code status;
            Pointer = OpenCL.clCreateBuffer(context.Pointer, memFlg, new IntPtr(size), dataPointer, &status);
            Size = size;
            Context = context;
            status.CheckError();
        }

        public Event MappingUnsafe(CommandQueue commandQueue, bool blocking, long offset, long size, out void* pointer)
        {
            var memFlg = cl_map_flags.CL_MAP_READ | cl_map_flags.CL_MAP_WRITE;

            cl_status_code status;
            void* event_ = null;
            pointer = OpenCL.clEnqueueMapBuffer(commandQueue.Pointer, Pointer, blocking, memFlg, new IntPtr(offset), new IntPtr(size), 0, null, &event_, &status);
            status.CheckError();
            return new Event(event_);
        }

        public Event UnMappingUnsafe(CommandQueue commandQueue, void* pointer)
        {
            void* event_ = null;
            OpenCL.clEnqueueUnmapMemObject(commandQueue.Pointer, Pointer, pointer, 0, null, &event_).CheckError();
            return new Event(event_);
        }

        private void CreateMappingMemory(Context context, long size)
        {
            cl_status_code status;
            Pointer = OpenCL.clCreateBuffer(context.Pointer, (cl_mem_flags.CL_MEM_ALLOC_HOST_PTR | cl_mem_flags.CL_MEM_READ_WRITE), new IntPtr(size), null, &status);
            Size = size;
            Context = context;
            status.CheckError();
        }
    }
}