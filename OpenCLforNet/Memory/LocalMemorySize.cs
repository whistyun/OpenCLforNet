using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCLforNet.Memory
{
    public readonly struct LocalMemorySize
    {
        public int Size { get; }
        public LocalMemorySize(int size)
        {
            Size = size;
        }
    }
}
