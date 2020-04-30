using System;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using OpenCLforNet.Function;

namespace OpenCLforNet.Memory
{
    public unsafe class SVMBuffer : AbstractBuffer
    {
        public long Size { get; }
        public Context Context { get; }
        public void* Pointer { get; }

        public SVMBuffer(Context context, long size, uint alignment = 0)
        {
            Size = size;
            Context = context;
            Pointer = OpenCL.clSVMAlloc(context.Pointer, cl_mem_flags.CL_MEM_READ_WRITE, new IntPtr(size), alignment);
        }

        public SVMBuffer(SVMBuffer origin)
        {
            Size = origin.Size;
            Context = origin.Context;
            Pointer = origin.Pointer;
        }

        public static Event Copy(CommandQueue commandQueue, void* src, int srcByteOffset, void* dst, int dstByteOffset, int byteSize, bool blocking)
        {
            void* event_ = null;
            int* test = null;

            var srcp = ((byte*)src) + srcByteOffset;
            var dstp = ((byte*)dst) + dstByteOffset;
            OpenCL.clEnqueueSVMMemcpy(commandQueue.Pointer, blocking, dstp, srcp, new IntPtr(byteSize), 0, null, &event_);
            return new Event(event_);
        }

        public static Event Copy(CommandQueue commandQueue, SVMBuffer src, int srcByteOffset, SVMBuffer dst, int dstByteOffset, int byteSize, bool blocking)
            => Copy(
                commandQueue,
                src.Pointer, srcByteOffset,
                dst.Pointer, dstByteOffset,
                byteSize,
                blocking
            );

        public Event CopyTo(CommandQueue commandQueue, int srcByteOffset, void* dst, int dstByteOffset, int byteSize, bool blocking)
        {
            return Copy(commandQueue, Pointer, srcByteOffset, dst, dstByteOffset, byteSize, blocking);
        }

        public Event CopyFrom(CommandQueue commandQueue, void* src, int srcByteOffset, int dstByteOffset, int byteSize, bool blocking)
        {
            return Copy(commandQueue, src, srcByteOffset, Pointer, dstByteOffset, byteSize, blocking);
        }

        public Event Mapping(CommandQueue commandQueue, bool blocking)
        {
            void* event_ = null;
            OpenCL.clEnqueueSVMMap(commandQueue.Pointer, blocking, (cl_map_flags.CL_MAP_READ | cl_map_flags.CL_MAP_WRITE), Pointer, new IntPtr(Size), 0, null, &event_).CheckError();
            return new Event(event_);
        }

        public Event UnMapping(CommandQueue commandQueue)
        {
            void* event_ = null;
            OpenCL.clEnqueueSVMUnmap(commandQueue.Pointer, Pointer, 0, null, &event_).CheckError();
            return new Event(event_);
        }

        protected override void DisposeManaged()
        {
        }

        protected override void DisposeUnManaged()
        {
            OpenCL.clSVMFree(Context.Pointer, Pointer);
        }
    }
}