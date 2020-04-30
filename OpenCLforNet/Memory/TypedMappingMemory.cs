using System;
using System.Collections.Generic;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using System.Collections;

namespace OpenCLforNet.Memory
{
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
}