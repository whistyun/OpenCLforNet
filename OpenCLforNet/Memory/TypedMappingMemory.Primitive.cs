using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;

namespace OpenCLforNet.Memory
{
    public unsafe class ByteMappingMemory : TypedMappingMemory<byte>
    {
        public ByteMappingMemory(Context context, long length) : base(context, length, sizeof(byte)) { }

        public ByteMappingMemory(Context context, byte[] data) : base(data.Length, sizeof(byte))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(byte) * data.Length);
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

        protected internal override byte GetAt(void* pointer, long index) => ((byte*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, byte value) => ((byte*)pointer)[index] = value;
    }

    public unsafe class CharMappingMemory : TypedMappingMemory<char>
    {
        public CharMappingMemory(Context context, long length) : base(context, length, sizeof(char)) { }

        public CharMappingMemory(Context context, char[] data) : base(data.Length, sizeof(char))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(char) * data.Length);
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

        protected internal override char GetAt(void* pointer, long index) => ((char*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, char value) => ((char*)pointer)[index] = value;
    }

    public unsafe class ShortMappingMemory : TypedMappingMemory<short>
    {
        public ShortMappingMemory(Context context, long length) : base(context, length, sizeof(short)) { }

        public ShortMappingMemory(Context context, short[] data) : base(data.Length, sizeof(short))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(short) * data.Length);
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

        protected internal override short GetAt(void* pointer, long index) => ((short*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, short value) => ((short*)pointer)[index] = value;
    }

    public unsafe class IntMappingMemory : TypedMappingMemory<int>
    {
        public IntMappingMemory(Context context, long length) : base(context, length, sizeof(int)) { }

        public IntMappingMemory(Context context, int[] data) : base(data.Length, sizeof(int))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(int) * data.Length);
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

        protected internal override int GetAt(void* pointer, long index) => ((int*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, int value) => ((int*)pointer)[index] = value;
    }

    public unsafe class LongMappingMemory : TypedMappingMemory<long>
    {
        public LongMappingMemory(Context context, long length) : base(context, length, sizeof(long)) { }

        public LongMappingMemory(Context context, long[] data) : base(data.Length, sizeof(long))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(long) * data.Length);
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

        protected internal override long GetAt(void* pointer, long index) => ((long*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, long value) => ((long*)pointer)[index] = value;
    }

    public unsafe class FloatMappingMemory : TypedMappingMemory<float>
    {
        public FloatMappingMemory(Context context, long length) : base(context, length, sizeof(float)) { }

        public FloatMappingMemory(Context context, float[] data) : base(data.Length, sizeof(float))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(float) * data.Length);
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

        protected internal override float GetAt(void* pointer, long index) => ((float*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, float value) => ((float*)pointer)[index] = value;
    }

    public unsafe class DoubleMappingMemory : TypedMappingMemory<double>
    {
        public DoubleMappingMemory(Context context, long length) : base(context, length, sizeof(double)) { }

        public DoubleMappingMemory(Context context, double[] data) : base(data.Length, sizeof(double))
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(double) * data.Length);
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

        protected internal override double GetAt(void* pointer, long index) => ((double*)pointer)[index];

        protected internal override void SetAt(void* pointer, long index, double value) => ((double*)pointer)[index] = value;
    }
}
