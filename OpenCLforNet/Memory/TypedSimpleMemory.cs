using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;

namespace OpenCLforNet.Memory
{
    public unsafe abstract class TypedSimpleMemory<T> : SimpleMemory, IArrayReadWrite<T> where T : struct
    {
        public int UnitSize { get; }

        public long Length { get; }

        protected TypedSimpleMemory(long length, int unitSize)
        {
            UnitSize = unitSize;
            Length = length;
        }

        protected TypedSimpleMemory(Context context, long length, int unitSize)
            : base(context, length * unitSize)
        {
            UnitSize = unitSize;
            Length = length;
        }

        public abstract Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList);

        public abstract Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList);
    }
}