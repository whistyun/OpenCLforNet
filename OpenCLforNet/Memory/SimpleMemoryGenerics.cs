using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;

namespace OpenCLforNet.Memory
{
    public unsafe class SimpleMemory<T> : SimpleMemory, IArrayReadWrite<T> where T : unmanaged
    {
        public int UnitSize => sizeof(T);

        public long Length { get; }

        public SimpleMemory(Context context, long length) : base(context, length * sizeof(T))
        {
            Length = length;
        }

        public SimpleMemory(Context context, T[] data) : this(context, data, data.Length) { }

        public SimpleMemory(Context context, T[] data, long length) : this(context, length)
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(T) * data.Length);
            }
        }

        public Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (T* ptr = data)
            {
                void* dataPointer = ptr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(T), length * sizeof(T),
                        dataPointer,
                        eventWaitList);
            }
        }

        public Event WriteDirect(
        CommandQueue commandQueue,
        bool blocking,
        long bufferOffset, int length,
        T[] data, int dataOffset,
        params Event[] eventWaitList)
        {
            fixed (T* ptr = data)
            {
                void* dataPointer = ptr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(T), length * sizeof(T),
                        dataPointer,
                        eventWaitList);
            }
        }
    }
}