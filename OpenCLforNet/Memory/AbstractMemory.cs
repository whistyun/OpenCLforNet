using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using OpenCLforNet.Function;

namespace OpenCLforNet.Memory
{
    public abstract unsafe class AbstractMemory : AbstractBuffer
    {
        public long Size { get; protected set; }
        public Context Context { get; protected set; }
        public void* Pointer { get; protected set; }

        public Event WriteUnsafe(CommandQueue commandQueue, bool blocking, IntPtr data, params Event[] eventWaitList)
            => WriteUnsafe(commandQueue, blocking, 0, Size, data, eventWaitList);

        public Event WriteUnsafe(CommandQueue commandQueue, bool blocking, long offset, long size, IntPtr data, params Event[] eventWaitList)
        {
            return WriteUnsafe(commandQueue, blocking, offset, size, (void*)data, eventWaitList);
        }

        public Event WriteUnsafe(CommandQueue commandQueue, bool blocking, void* data, params Event[] eventWaitList)
            => WriteUnsafe(commandQueue, blocking, 0, Size, data, eventWaitList);

        public Event WriteUnsafe(CommandQueue commandQueue, bool blocking, long offset, long size, void* data, params Event[] eventWaitList)
        {
            void* event_ = null;

            var num = (uint)eventWaitList.Length;
            var list = eventWaitList.Select(e => new IntPtr(e.Pointer)).ToArray();
            fixed (void* listPointer = list)
            {
                OpenCL.clEnqueueWriteBuffer(commandQueue.Pointer, Pointer, blocking, new IntPtr(offset), new IntPtr(size), data, num, listPointer, &event_).CheckError();
            }

            return new Event(event_);
        }

        public Event ReadUnsafe(CommandQueue commandQueue, bool blocking, IntPtr data, params Event[] eventWaitList)
            => ReadUnsafe(commandQueue, blocking, data, 0, Size, eventWaitList);

        public Event ReadUnsafe(CommandQueue commandQueue, bool blocking, IntPtr data, long offset, long size, params Event[] eventWaitList)
        {
            return ReadUnsafe(commandQueue, blocking, offset, size, (void*)data, eventWaitList);
        }

        public Event ReadUnsafe(CommandQueue commandQueue, bool blocking, void* data, params Event[] eventWaitList)
            => ReadUnsafe(commandQueue, blocking, 0, Size, data, eventWaitList);

        public Event ReadUnsafe(CommandQueue commandQueue, bool blocking, long offset, long size, void* data, params Event[] eventWaitList)
        {
            void* event_ = null;

            var num = (uint)eventWaitList.Length;
            var list = eventWaitList.Select(e => new IntPtr(e.Pointer)).ToArray();
            fixed (void* listPointer = list)
            {
                OpenCL.clEnqueueReadBuffer(commandQueue.Pointer, Pointer, blocking, new IntPtr(offset), new IntPtr(size), data, num, listPointer, &event_).CheckError();
            }

            return new Event(event_);
        }

        public Event Copy(CommandQueue commandQueue, long srcOffset, long dstOffset, long size, params Event[] eventWaitList)
        {
            return CopyFrom(commandQueue, this, srcOffset, dstOffset, size, eventWaitList);
        }

        public Event CopyFrom(CommandQueue commandQueue, AbstractMemory memory, long srcOffset = 0, long dstOffset = 0, long? size = null, params Event[] eventWaitList)
        {
            void* event_ = null;

            var num = (uint)eventWaitList.Length;
            var list = eventWaitList.Select(e => new IntPtr(e.Pointer)).ToArray();
            fixed (void* listPointer = list)
            {
                OpenCL.clEnqueueCopyBuffer(commandQueue.Pointer, Pointer, memory.Pointer, new IntPtr(srcOffset), new IntPtr(dstOffset), new IntPtr(size ?? Size), num, listPointer, &event_).CheckError();
            }

            return new Event(event_);
        }

        protected override void DisposeManaged()
        {
        }

        protected override void DisposeUnManaged()
        {
            OpenCL.clReleaseMemObject(Pointer).CheckError();
        }
    }
}
