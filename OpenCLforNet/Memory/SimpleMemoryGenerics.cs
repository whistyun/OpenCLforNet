using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;

namespace OpenCLforNet.Memory
{
    /// <summary>
    /// OpenCL memory object(Typed in managed code)
    /// </summary>
    /// <typeparam name="T">Any unmanaged type(value/struct)</typeparam>
    public unsafe class SimpleMemory<T> : SimpleMemory, IArrayReadWrite<T> where T : unmanaged
    {
        /// <summary>
        /// sizeof(T)
        /// </summary>
        public int UnitSize => sizeof(T);
        /// <summary>
        /// The number of <c>T</c> elements
        /// </summary>
        public long Length { get; }
        ///<inheritdoc cref="SimpleMemory(Context, long)"/>
        /// <param name="context">OpenCL Context</param>
        /// <param name="length">size in the number of <c>T</c> elements</param>
        public SimpleMemory(Context context, long length) : base(context, length * sizeof(T))
        {
            Length = length;
        }
        ///<inheritdoc cref="SimpleMemory{T}(Context, T[])"/>
        public SimpleMemory(Context context, T[] data) : this(context, data, data.Length) { }
        /// <inheritdoc cref="SimpleMemory(Context, void*, long)"/>
        /// <param name="context">OpenCL context</param>
        /// <param name="data">Data to copy to the buffer</param>
        /// <param name="length">size of <c>data</c> in the number of <c>T</c> elements</param>
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