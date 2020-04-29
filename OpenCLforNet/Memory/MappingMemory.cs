using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using OpenCLforNet.Function;
using System.Collections;

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

    public unsafe abstract class TypedMappingMemory<T> : MappingMemory, IArrayReadWrite<T> where T : struct
    {
        public int UnitSize { get; }

        public long Length { get; }

        protected TypedMappingMemory(long length, int unitSize)
        {
            UnitSize = unitSize;
            Length = length;
        }

        protected TypedMappingMemory(Context context, long length, int unitSize)
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

        protected internal abstract T GetAt(void* pointer, long idx);
        protected internal abstract void SetAt(void* pointer, long idx, T value);

        public TypedMap<T> Mapping(CommandQueue commandQueue, bool blocking, long offset, long length)
        {
            return new TypedMap<T>(commandQueue, this, blocking, offset, length);
        }

    }

    public unsafe class TypedMap<T> : IDisposable, IEnumerable<T> where T : struct
    {
        private bool isDisposed = false;
        private CommandQueue queue;
        private TypedMappingMemory<T> owner;

        private bool pointerConstructored;
        public readonly void* Pointer;
        public long Length { get; }

        public TypedMap(CommandQueue queue, TypedMappingMemory<T> owner, bool blocking, long offset, long length)
        {
            this.queue = queue;
            this.owner = owner;
            this.Length = length;

            owner.MappingUnsafe(
                queue, blocking,
                offset * owner.UnitSize, length * owner.UnitSize,
                out this.Pointer).Wait();
            pointerConstructored = true;
        }

        ~TypedMap() => Dispose(false);

        public T this[long index]
        {
            set
            {
                CheckIndex(index);
                owner.SetAt(Pointer, index, value);
            }
            get
            {
                CheckIndex(index);
                return owner.GetAt(Pointer, index);
            }
        }

        private void CheckIndex(long idx)
        {
            if (idx < 0 || idx >= Length)
                throw new IndexOutOfRangeException();
        }

        public IEnumerator<T> GetEnumerator() => new MappedMemoryEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool includeManaged)
        {
            if (!isDisposed)
            {
                if (pointerConstructored)
                    owner.UnMappingUnsafe(queue, Pointer).Wait();

                isDisposed = true;
            }
        }
    }

    public class MappedMemoryEnumerator<T> : IEnumerator<T> where T : struct
    {
        public long Index { get; private set; }
        public long Length { get; private set; }
        public TypedMap<T> Buffer { get; private set; }

        public MappedMemoryEnumerator(TypedMap<T> buffer)
        {
            Index = -1;
            Buffer = buffer;
            Length = buffer.Length;
        }

        public T Current { get => Buffer[Index]; }

        object IEnumerator.Current => this.Current;

        public bool MoveNext() => ++Index < Length;

        public void Reset() => Index = -1;

        public void Dispose() { }
    }

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
