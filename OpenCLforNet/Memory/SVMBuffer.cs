using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using OpenCLforNet.Function;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Runtime.InteropServices;

namespace OpenCLforNet.Memory
{
    public unsafe class SVMBuffer : AbstractBuffer
    {
        public long Size { get; }
        public Context Context { get; }
        public void* Pointer { get; }

        public SVMBuffer(Context context, long size, uint alignment = 0)
        {
            Size = size;
            Context = context;
            Pointer = OpenCL.clSVMAlloc(context.Pointer, cl_mem_flags.CL_MEM_READ_WRITE, new IntPtr(size), alignment);
        }

        public SVMBuffer(SVMBuffer origin)
        {
            Size = origin.Size;
            Context = origin.Context;
            Pointer = origin.Pointer;
        }

        public static Event Copy(CommandQueue commandQueue, void* src, int srcByteOffset, void* dst, int dstByteOffset, int byteSize, bool blocking)
        {
            void* event_ = null;
            int* test = null;

            var srcp = ((byte*)src) + srcByteOffset;
            var dstp = ((byte*)dst) + dstByteOffset;
            OpenCL.clEnqueueSVMMemcpy(commandQueue.Pointer, blocking, dstp, srcp, new IntPtr(byteSize), 0, null, &event_);
            return new Event(event_);
        }

        public static Event Copy(CommandQueue commandQueue, SVMBuffer src, int srcByteOffset, SVMBuffer dst, int dstByteOffset, int byteSize, bool blocking)
            => Copy(
                commandQueue,
                src.Pointer, srcByteOffset,
                dst.Pointer, dstByteOffset,
                byteSize,
                blocking
            );

        public Event CopyTo(CommandQueue commandQueue, int srcByteOffset, void* dst, int dstByteOffset, int byteSize, bool blocking)
        {
            return Copy(commandQueue, Pointer, srcByteOffset, dst, dstByteOffset, byteSize, blocking);
        }

        public Event CopyFrom(CommandQueue commandQueue, void* src, int srcByteOffset, int dstByteOffset, int byteSize, bool blocking)
        {
            return Copy(commandQueue, src, srcByteOffset, Pointer, dstByteOffset, byteSize, blocking);
        }

        public Event Mapping(CommandQueue commandQueue, bool blocking)
        {
            void* event_ = null;
            OpenCL.clEnqueueSVMMap(commandQueue.Pointer, blocking, (cl_map_flags.CL_MAP_READ | cl_map_flags.CL_MAP_WRITE), Pointer, new IntPtr(Size), 0, null, &event_).CheckError();
            return new Event(event_);
        }

        public Event UnMapping(CommandQueue commandQueue)
        {
            void* event_ = null;
            OpenCL.clEnqueueSVMUnmap(commandQueue.Pointer, Pointer, 0, null, &event_).CheckError();
            return new Event(event_);
        }

        protected override void DisposeManaged()
        {
        }

        protected override void DisposeUnManaged()
        {
            OpenCL.clSVMFree(Context.Pointer, Pointer);
        }
    }

    public unsafe abstract class TypedSVMBuffer<T> : SVMBuffer, IEnumerable<T> where T : struct
    {
        public int UnitSize { get; }

        public int Length { get => LengthX * LengthY * LengthZ; }

        public int LengthX { get; }

        public int LengthY { get; }

        public int LengthZ { get; }

        protected TypedSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment, int unitSize)
            : base(context, lengthX * lengthY * lengthZ * unitSize, alignment)
        {
            UnitSize = unitSize;
            LengthX = lengthX;
            LengthY = lengthY;
            LengthZ = lengthZ;
        }

        protected TypedSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ, int unitSize) : base(origin)
        {
            UnitSize = unitSize;
            LengthX = lengthX;
            LengthY = lengthY;
            LengthZ = lengthZ;
        }

        public T this[int index]
        {
            get
            {
                CheckIndex(index);
                return GetAt(index);
            }
            set
            {
                CheckIndex(index);
                SetAt(index, value);
            }
        }

        public T this[int indexX, int indexY]
        {
            get
            {
                CheckIndex(indexX, indexY);
                return GetAt(indexX + LengthX * indexY);
            }
            set
            {
                CheckIndex(indexX, indexY);
                SetAt(indexX + LengthX * indexY, value);

            }
        }

        public T this[int indexX, int indexY, int indexZ]
        {
            get
            {
                CheckIndex(indexX, indexY, indexZ);
                return GetAt(indexX + LengthX * indexY + LengthX * LengthY * indexZ);
            }
            set
            {
                CheckIndex(indexX, indexY, indexZ);
                SetAt(indexX + LengthX * indexY + LengthX * LengthY * indexZ, value);
            }
        }

        public abstract T GetAt(int idx);
        public abstract void SetAt(int idx, T val);

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= Length) throw new IndexOutOfRangeException(nameof(index));
        }

        private void CheckIndex(int indexX, int indexY)
        {
            if (indexX < 0 || indexX >= LengthX) throw new IndexOutOfRangeException(nameof(indexX));
            if (indexY < 0 || indexY >= LengthX) throw new IndexOutOfRangeException(nameof(indexX));
        }

        private void CheckIndex(int indexX, int indexY, int indexZ)
        {
            if (indexX < 0 || indexX >= LengthX) throw new IndexOutOfRangeException(nameof(indexX));
            if (indexY < 0 || indexY >= LengthX) throw new IndexOutOfRangeException(nameof(indexX));
            if (indexZ < 0 || indexZ >= LengthX) throw new IndexOutOfRangeException(nameof(indexX));
        }

        public IEnumerator<T> GetEnumerator() => new SVMEnumerator<T>(this);

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class SVMEnumerator<T> : IEnumerator<T> where T : struct
    {
        public int Index { get; private set; }
        public int Length { get; private set; }
        public TypedSVMBuffer<T> Buffer { get; private set; }

        public SVMEnumerator(TypedSVMBuffer<T> buffer)
        {
            Index = -1;
            Buffer = buffer;
            Length = buffer.LengthX * buffer.LengthY * buffer.LengthZ;
        }

        public T Current { get => Buffer.GetAt(Index); }

        object IEnumerator.Current => this.Current;

        public bool MoveNext() => ++Index < Length;

        public void Reset() => Index = -1;

        public void Dispose() { }
    }

    public unsafe class ByteSVMBuffer : TypedSVMBuffer<byte>
    {
        public ByteSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(byte)) { }

        public ByteSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(byte)) { }

        public override byte GetAt(int index) => ((byte*)Pointer)[index];
        public override void SetAt(int index, byte value) => ((byte*)Pointer)[index] = value;
    }
    public unsafe class CharSVMBuffer : TypedSVMBuffer<char>
    {
        public CharSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(char)) { }

        public CharSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(char)) { }

        public override char GetAt(int index) => ((char*)Pointer)[index];
        public override void SetAt(int index, char value) => ((char*)Pointer)[index] = value;
    }
    public unsafe class ShortSVMBuffer : TypedSVMBuffer<short>
    {
        public ShortSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(short)) { }

        public ShortSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(short)) { }

        public override short GetAt(int index) => ((short*)Pointer)[index];
        public override void SetAt(int index, short value) => ((short*)Pointer)[index] = value;
    }
    public unsafe class IntSVMBuffer : TypedSVMBuffer<int>
    {
        public IntSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(int)) { }

        public IntSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(int)) { }

        public override int GetAt(int index) => ((int*)Pointer)[index];
        public override void SetAt(int index, int value) => ((int*)Pointer)[index] = value;
    }
    public unsafe class LongSVMBuffer : TypedSVMBuffer<long>
    {
        public LongSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(long)) { }

        public LongSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(long)) { }

        public override long GetAt(int index) => ((long*)Pointer)[index];
        public override void SetAt(int index, long value) => ((long*)Pointer)[index] = value;
    }
    public unsafe class FloatSVMBuffer : TypedSVMBuffer<float>
    {
        public FloatSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(float)) { }

        public FloatSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(float)) { }

        public override float GetAt(int index) => ((float*)Pointer)[index];
        public override void SetAt(int index, float value) => ((float*)Pointer)[index] = value;
    }
    public unsafe class DoubleSVMBuffer : TypedSVMBuffer<double>
    {
        public DoubleSVMBuffer(Context context, int lengthX, int lengthY, int lengthZ, uint alignment)
            : base(context, lengthX, lengthY, lengthZ, alignment, sizeof(double)) { }

        public DoubleSVMBuffer(SVMBuffer origin, int lengthX, int lengthY, int lengthZ)
            : base(origin, lengthX, lengthY, lengthZ, sizeof(double)) { }

        public override double GetAt(int index) => ((double*)Pointer)[index];
        public override void SetAt(int index, double value) => ((double*)Pointer)[index] = value;
    }
}
