using System;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using System.Runtime.InteropServices;

namespace OpenCLforNet.Memory
{
    public unsafe class StructMappingMemory<T> : TypedMappingMemory<T> where T : struct
    {
        public StructMappingMemory(Context context, long length)
            : base(context, length, Marshal.SizeOf(typeof(T))) { }

        public override Event ReadDirect(CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            IntPtr ptr = Marshal.AllocHGlobal(UnitSize * length);
            try
            {
                var ev = ReadUnsafe(
                        commandQueue, blocking,
                        bufferOffset * UnitSize, length * UnitSize,
                        ptr,
                        eventWaitList).Wait();

                IntPtr elmOffset = ptr;
                for (var i = 0; i < length; ++i)
                {
                    data[dataOffset + i] = Marshal.PtrToStructure<T>(elmOffset);
                    elmOffset += UnitSize;
                }

                return ev;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            IntPtr ptr = Marshal.AllocHGlobal(UnitSize * length);
            try
            {
                IntPtr elmOffset = ptr;
                for (var i = 0; i < length; ++i)
                {
                    Marshal.StructureToPtr(data[dataOffset + i], elmOffset, false);
                    elmOffset += UnitSize;
                }

                return WriteUnsafe(
                        commandQueue, blocking,
                        bufferOffset * UnitSize, length * UnitSize,
                        ptr,
                        eventWaitList).Wait();
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        protected internal override T GetAt(void* pointer, long index)
        {
            byte* stPtr = ((byte*)pointer) + (index * UnitSize);
            return Marshal.PtrToStructure<T>(new IntPtr(stPtr));
        }

        protected internal override void SetAt(void* pointer, long index, T value)
        {
            byte* stPtr = ((byte*)pointer) + (index * UnitSize);
            Marshal.StructureToPtr<T>(value, new IntPtr(stPtr), false);
        }
    }
}
