using OpenCLforNet.PlatformLayer;

namespace OpenCLforNet.Memory
{
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
