using OpenCLforNet.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OpenCLforNet.Memory
{
    public interface IArrayReadWrite<T>
    {
        long Length { get; }

        Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList);

        Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList);
    }

    public static class IArrayReadWriteExt
    {
        public static Event Write<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                T[] data,
                params Event[] eventWaitList)
            => Write(buffer, commandQueue, blocking, 0L, data.Length, data, 0, eventWaitList);

        public static Event Write<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                int length,
                T[] data,
                params Event[] eventWaitList)
            => Write(buffer, commandQueue, blocking, 0L, length, data, 0, eventWaitList);

        public static Event Write<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data,
                params Event[] eventWaitList)
            => Write(buffer, commandQueue, blocking, bufferOffset, length, data, 0, eventWaitList);

        public static Event Write<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            if (bufferOffset < 0 || bufferOffset + length > buffer.Length)
                throw new IndexOutOfRangeException($"{nameof(bufferOffset)}: {bufferOffset}, {nameof(length)}: {length}");

            if (dataOffset < 0 || dataOffset + length > data.Length)
                throw new IndexOutOfRangeException($"{nameof(dataOffset)}: {dataOffset}, {nameof(length)}: {length}");

            return buffer.WriteDirect(
                    commandQueue,
                    blocking,
                    bufferOffset, length,
                    data, dataOffset,
                    eventWaitList);
        }

        public static Event Read<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                T[] data,
                params Event[] eventWaitList)
            => Read(buffer, commandQueue, blocking, 0L, data.Length, data, 0, eventWaitList);

        public static Event Read<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                int length,
                T[] data,
                params Event[] eventWaitList)
            => Read(buffer, commandQueue, blocking, 0L, length, data, 0, eventWaitList);

        public static Event Read<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data,
                params Event[] eventWaitList)
            => Read(buffer, commandQueue, blocking, bufferOffset, length, data, 0, eventWaitList);

        public static Event Read<T>(
                this IArrayReadWrite<T> buffer,
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                T[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            if (bufferOffset < 0 || bufferOffset + length > buffer.Length)
                throw new IndexOutOfRangeException($"{nameof(bufferOffset)}: {bufferOffset}, {nameof(length)}: {length}");

            if (dataOffset < 0 || dataOffset + length > data.Length)
                throw new IndexOutOfRangeException($"{nameof(dataOffset)}: {dataOffset}, {nameof(length)}: {length}");

            return buffer.ReadDirect(
                    commandQueue,
                    blocking,
                    bufferOffset, length,
                    data, dataOffset,
                    eventWaitList);
        }
    }
}
