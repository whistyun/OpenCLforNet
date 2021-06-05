using System;
using System.Collections.Generic;
using System.Text;

namespace OpenCLforNet.Memory
{
    /// <summary>
    /// Local memory size definition
    /// </summary>
    public readonly struct LocalMemorySize
    {
        /// <summary>
        /// Size in bytes
        /// </summary>
        public int Size { get; }
        /// <inheritdoc cref="LocalMemorySize"/>
        /// <param name="size">Size in bytes</param>
        public LocalMemorySize(int size)
        {
            Size = size;
        }
    }
}
