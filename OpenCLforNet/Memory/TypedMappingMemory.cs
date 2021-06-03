using System;
using System.Collections.Generic;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using System.Collections;

namespace OpenCLforNet.Memory
{
    public unsafe class TypedMappingMemory<T> : MappingMemory, IArrayReadWrite<T> where T : unmanaged
    {
        public int UnitSize => sizeof(T);

        public long Length { get; }

        public TypedMappingMemory(Context context, long length) : base(context, length * sizeof(T))
        {
            Length = length;
        }

        public TypedMappingMemory(Context context, T[] data) : this(context, data, data.Length) { }

        public TypedMappingMemory(Context context, T[] data, long length) : this(context, length)
        {
            fixed (void* dataPtr = data)
            {
                CreateMappingMemory(context, dataPtr, sizeof(T) * length);
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

        protected internal T GetAt(void* pointer, long index) => ((T*)pointer)[index];
        protected internal void SetAt(void* pointer, long index, T value) => ((T*)pointer)[index] = value;

        public TypedMap<T> Mapping(CommandQueue commandQueue, bool blocking) => Mapping(commandQueue, blocking, 0, Length);

        public TypedMap<T> Mapping(CommandQueue commandQueue, bool blocking, long offset, long length)
        {
            return new TypedMap<T>(commandQueue, this, blocking, offset, length);
        }

    }

    public unsafe class TypedMap<T> : IDisposable, IEnumerable<T> where T : unmanaged
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

    public class MappedMemoryEnumerator<T> : IEnumerator<T> where T : unmanaged
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
}