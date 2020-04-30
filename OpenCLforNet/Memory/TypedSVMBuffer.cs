using System;
using System.Collections.Generic;
using OpenCLforNet.PlatformLayer;
using System.Collections;

namespace OpenCLforNet.Memory
{
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
}