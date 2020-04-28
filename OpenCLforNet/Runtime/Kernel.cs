using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using OpenCLforNet.Memory;
using OpenCLforNet.Function;

namespace OpenCLforNet.Runtime
{
    public unsafe class Kernel : IDisposable
    {
        private bool isDisposed = false;

        public string KernelName { get; }
        public CLProgram Program { get; }
        public void* Pointer { get; }

        public Kernel(CLProgram program, string kernelName)
        {
            KernelName = kernelName;
            Program = program;

            var status = cl_status_code.CL_SUCCESS;
            var kernelNameArray = Encoding.UTF8.GetBytes(kernelName);
            fixed (byte* kernelNameArrayPointer = kernelNameArray)
            {
                Pointer = OpenCL.clCreateKernel(program.Pointer, kernelNameArrayPointer, &status);
                status.CheckError();
            }
        }

        ~Kernel() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool includeManaged)
        {
            if (!isDisposed)
            {
                if (includeManaged)
                    DisposeManaged();

                DisposeUnManaged();
                isDisposed = true;
            }
        }
        private readonly Dictionary<int, IntPtr> Args = new Dictionary<int, IntPtr>();

        public void SetArg(int index, object arg)
        {
            if (arg == null)
            {
                throw new NullReferenceException("Arg must not be null.");
            }
            else if (arg is AbstractMemory mem)
            {
                // Remove old memobject
                if (Args.ContainsKey(index))
                {
                    Marshal.FreeCoTaskMem(Args[index]);
                    Args.Remove(index);
                }

                // Add new memobject
                var argPointer = Marshal.AllocCoTaskMem(IntPtr.Size);
                Marshal.WriteIntPtr(argPointer, new IntPtr(mem.Pointer));
                OpenCL.clSetKernelArg(Pointer, index, IntPtr.Size, (void*)argPointer).CheckError();
                Args.Add(index, argPointer);
            }
            else if (arg is SVMBuffer buf)
            {
                OpenCL.clSetKernelArgSVMPointer(Pointer, index, buf.Pointer).CheckError();
            }
            else if (arg is byte barg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(byte), &barg).CheckError();
            }
            else if (arg is sbyte sbarg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(sbyte), &sbarg).CheckError();
            }
            else if (arg is char carg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(char), &carg).CheckError();
            }
            else if (arg is short sarg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(short), &sarg).CheckError();
            }
            else if (arg is ushort usarg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(ushort), &usarg).CheckError();
            }
            else if (arg is int iarg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(int), &iarg).CheckError();
            }
            else if (arg is uint uiarg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(uint), &uiarg).CheckError();
            }
            else if (arg is long larg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(long), &larg).CheckError();
            }
            else if (arg is ulong ularg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(ulong), &ularg).CheckError();
            }
            else if (arg is float farg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(float), &farg).CheckError();
            }
            else if (arg is double darg)
            {
                OpenCL.clSetKernelArg(Pointer, index, sizeof(double), &darg).CheckError();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void SetArgs(params object[] args)
        {
            for (var i = 0; i < args.Length; i++)
            {
                SetArg(i, args[i]);
            }
        }

        private uint Dimention = 1;
        private readonly IntPtr* WorkSizes = (IntPtr*)Marshal.AllocCoTaskMem(3 * IntPtr.Size);

        public void SetWorkSize(params long[] workSizes)
        {
            if (workSizes.Length <= 0 || 4 <= workSizes.Length)
                throw new ArgumentException("workSizes length is invalid.");

            Dimention = (uint)workSizes.Length;
            for (var i = 0; i < Dimention; i++)
                WorkSizes[i] = new IntPtr(workSizes[i]);
        }

        public Event NDRange(CommandQueue commandQueue, params Event[] eventWaitList)
        {
            void* event_ = null;

            var num = (uint)eventWaitList.Length;
            var list = eventWaitList.Select(e => new IntPtr(e.Pointer)).ToArray();
            fixed (void* listPointer = list)
            {
                OpenCL.clEnqueueNDRangeKernel(commandQueue.Pointer, Pointer, Dimention, null, WorkSizes, null, num, listPointer, &event_).CheckError();
            }

            return new Event(event_);
        }

        public Event NDRange(CommandQueue commandQueue, long[] workSizes)
        {
            SetWorkSize(workSizes);
            return NDRange(commandQueue);
        }

        public Event NDRange(CommandQueue commandQueue, long[] workSizes, params object[] args)
        {
            SetWorkSize(workSizes);
            SetArgs(args);
            return NDRange(commandQueue);
        }

        protected void DisposeUnManaged()
        {
            foreach (var arg in Args)
            {
                Marshal.FreeCoTaskMem(arg.Value);
            }
            OpenCL.clReleaseKernel(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
