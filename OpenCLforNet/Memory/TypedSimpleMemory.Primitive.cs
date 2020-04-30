using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;

namespace OpenCLforNet.Memory
{
    public unsafe class ByteSimpleMemory : TypedSimpleMemory<byte>
    {
        public ByteSimpleMemory(Context context, long length) : base(context, length, sizeof(byte)) { }

        public ByteSimpleMemory(Context context, byte[] data) : base(data.Length, sizeof(byte))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(byte) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                byte[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (byte* bytePtr = data)
            {
                void* dataPointer = bytePtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(byte), length * sizeof(byte),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                byte[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (byte* bytePtr = data)
            {
                void* dataPointer = bytePtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(byte), length * sizeof(byte),
                        dataPointer,
                        eventWaitList);
            }
        }
    }

    public unsafe class CharSimpleMemory : TypedSimpleMemory<char>
    {
        public CharSimpleMemory(Context context, long length) : base(context, length, sizeof(char)) { }

        public CharSimpleMemory(Context context, char[] data) : base(data.Length, sizeof(char))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(char) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                char[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (char* charPtr = data)
            {
                void* dataPointer = charPtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(char), length * sizeof(char),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                char[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (char* charPtr = data)
            {
                void* dataPointer = charPtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(char), length * sizeof(char),
                        dataPointer,
                        eventWaitList);
            }
        }
    }

    public unsafe class ShortSimpleMemory : TypedSimpleMemory<short>
    {
        public ShortSimpleMemory(Context context, long length) : base(context, length, sizeof(short)) { }

        public ShortSimpleMemory(Context context, short[] data) : base(data.Length, sizeof(short))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(short) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                short[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (short* shortPtr = data)
            {
                void* dataPointer = shortPtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(short), length * sizeof(short),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                short[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (short* shortPtr = data)
            {
                void* dataPointer = shortPtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(short), length * sizeof(short),
                        dataPointer,
                        eventWaitList);
            }
        }
    }

    public unsafe class IntSimpleMemory : TypedSimpleMemory<int>
    {
        public IntSimpleMemory(Context context, long length) : base(context, length, sizeof(int)) { }

        public IntSimpleMemory(Context context, int[] data) : base(data.Length, sizeof(int))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(int) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                int[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (int* intPtr = data)
            {
                void* dataPointer = intPtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(int), length * sizeof(int),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                int[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (int* intPtr = data)
            {
                void* dataPointer = intPtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(int), length * sizeof(int),
                        dataPointer,
                        eventWaitList);
            }
        }
    }

    public unsafe class LongSimpleMemory : TypedSimpleMemory<long>
    {
        public LongSimpleMemory(Context context, long length) : base(context, length, sizeof(long)) { }

        public LongSimpleMemory(Context context, long[] data) : base(data.Length, sizeof(long))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(long) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                long[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (long* longPtr = data)
            {
                void* dataPointer = longPtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(long), length * sizeof(long),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                long[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (long* longPtr = data)
            {
                void* dataPointer = longPtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(long), length * sizeof(long),
                        dataPointer,
                        eventWaitList);
            }
        }
    }

    public unsafe class FloatSimpleMemory : TypedSimpleMemory<float>
    {
        public FloatSimpleMemory(Context context, long length) : base(context, length, sizeof(float)) { }

        public FloatSimpleMemory(Context context, float[] data) : base(data.Length, sizeof(float))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(float) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                float[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (float* floatPtr = data)
            {
                void* dataPointer = floatPtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(float), length * sizeof(float),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                float[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (float* floatPtr = data)
            {
                void* dataPointer = floatPtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(float), length * sizeof(float),
                        dataPointer,
                        eventWaitList);
            }
        }
    }

    public unsafe class DoubleSimpleMemory : TypedSimpleMemory<double>
    {
        public DoubleSimpleMemory(Context context, long length) : base(context, length, sizeof(double)) { }

        public DoubleSimpleMemory(Context context, double[] data) : base(data.Length, sizeof(double))
        {
            fixed (void* dataPtr = data)
            {
                CreateSimpleMemory(context, dataPtr, sizeof(double) * data.Length);
            }
        }

        public override Event ReadDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                double[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (double* doublePtr = data)
            {
                void* dataPointer = doublePtr + dataOffset;
                return ReadUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(double), length * sizeof(double),
                        dataPointer,
                        eventWaitList);
            }
        }

        public override Event WriteDirect(
                CommandQueue commandQueue,
                bool blocking,
                long bufferOffset, int length,
                double[] data, int dataOffset,
                params Event[] eventWaitList)
        {
            fixed (double* doublePtr = data)
            {
                void* dataPointer = doublePtr + dataOffset;
                return WriteUnsafe(
                        commandQueue,
                        blocking,
                        bufferOffset * sizeof(double), length * sizeof(double),
                        dataPointer,
                        eventWaitList);
            }
        }
    }
}
