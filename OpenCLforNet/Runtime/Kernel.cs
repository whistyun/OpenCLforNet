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
        /// <summary>
        /// clSetKernelArg(this, index, sizeof(T), &amp;arg)
        /// </summary>
        /// <typeparam name="T">Any unmanaged type (value/struct)</typeparam>
        /// <param name="index">Index of this argument</param>
        /// <param name="arg">Argument to set</param>
        public void SetArg<T>(int index, T arg) where T:unmanaged
        {
            OpenCL.clSetKernelArg(Pointer, index, sizeof(T), &arg).CheckError();
        }
        /// <summary>
        /// clSetKernelArgSVMPointer(this, index, buf)
        /// </summary>
        /// <typeparam name="T">Any unmanaged type (value/struct)</typeparam>
        /// <param name="index">Index of this argument</param>
        /// <param name="buf">SVM to set</param>
        public void SetArg(int index, SVMBuffer buf)
        {
            OpenCL.clSetKernelArgSVMPointer(Pointer, index, buf.Pointer).CheckError();
        }
        /// <summary>
        /// clSetKernelArg(this, index, mem)
        /// </summary>
        /// <param name="index">Index of this argument</param>
        /// <param name="mem">Memory object to set</param>
        public void SetArg(int index, AbstractMemory mem)
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
        /// <summary>
        /// clSetKernelArg(this, index, size.Size, null)
        /// </summary>
        /// <param name="index">Index of this argument</param>
        /// <param name="size">LocalMemorySize object</param>
        public void SetArg(int index, LocalMemorySize size)
        {
            OpenCL.clSetKernelArg(Pointer, index, size.Size, null).CheckError();
        }
        public void SetArg(int index, object arg)
        {
            switch (arg)
            {
                case null:
                    throw new NullReferenceException("Arg must not be null.");
                case AbstractMemory mem:
                    SetArg(index, mem);
                    break;
                case SVMBuffer buf:
                    SetArg(index, buf);
                    break;
                case LocalMemorySize size:
                    SetArg(index, size);
                    break;
                case byte arg_:
                    SetArg(index, arg_);
                    break;
                case sbyte arg_:
                    SetArg(index, arg_);
                    break;
                case char arg_:
                    SetArg(index, arg_);
                    break;
                case short arg_:
                    SetArg(index, arg_);
                    break;
                case ushort arg_:
                    SetArg(index, arg_);
                    break;
                case int arg_:
                    SetArg(index, arg_);
                    break;
                case uint arg_:
                    SetArg(index, arg_);
                    break;
                case long arg_:
                    SetArg(index, arg_);
                    break;
                case ulong arg_:
                    SetArg(index, arg_);
                    break;
                case float arg_:
                    SetArg(index, arg_);
                    break;
                case double arg_:
                    SetArg(index, arg_);
                    break;
                default:
                    throw new ArgumentException($"Type of Arg was {arg.GetType()}");
            }
        }
        /// <summary>
        /// Set multiple arguments with SetArg()
        /// </summary>
        /// <param name="args">Argments. Set null to skip setting nth argument.</param>
        public void SetArgs(params object[] args)
        {
            for(int index = 0; index < args.Length; index++)
            {
                var arg = args[index];
                if(arg is null) { continue; }
                SetArg(index, arg);
            }
        }

        private uint Dimension = 1;
        private readonly IntPtr* WorkSizes = (IntPtr*)Marshal.AllocCoTaskMem(3 * IntPtr.Size);

        private uint localDimension = 1;
        private bool localSizeSet;
        private readonly IntPtr* LocalSizes = (IntPtr*)Marshal.AllocCoTaskMem(3 * IntPtr.Size);

        public void SetLocalSize(params long[] localSizes)
        {
            if (localSizes is null || localSizes.Length == 0)
            {
                localSizeSet = false;
            }
            else
            {
                if (localSizes.Length <= 0 || 4 <= localSizes.Length)
                    throw new ArgumentException("localSizes length is invalid.");

                localDimension = (uint)localSizes.Length;
                for (var i = 0; i < localDimension; i++)
                    LocalSizes[i] = new IntPtr(localSizes[i]);

                localSizeSet = true;
            }
        }

        public void SetWorkSize(params long[] workSizes)
        {
            if (workSizes.Length <= 0 || 4 <= workSizes.Length)
                throw new ArgumentException("workSizes length is invalid.");

            Dimension = (uint)workSizes.Length;
            for (var i = 0; i < Dimension; i++)
                WorkSizes[i] = new IntPtr(workSizes[i]);
        }

        /// <summary>
        /// clEnqueueNDRangeKernel(commandQueue, this, Dimemsion, null, WorkSizes, LocalSizes, eventWaitList.Length, eventWaitList, &amp;event_)
        /// </summary>
        /// <param name="commandQueue">CommandQueue to enqueue this job</param>
        /// <param name="eventWaitList">Events to wait before starting this job</param>
        /// <returns></returns>
        public Event NDRange(CommandQueue commandQueue, params Event[] eventWaitList)
        {
            void* event_ = null;

            var num = (uint)eventWaitList.Length;
            var list = eventWaitList.Select(e => new IntPtr(e.Pointer)).ToArray();
            fixed (void* listPointer = list)
            {
                cl_status_code stcd;

                if (localSizeSet)
                {
                    stcd = OpenCL.clEnqueueNDRangeKernel(
                            commandQueue.Pointer,
                            Pointer,
                            Dimension, null, WorkSizes, LocalSizes,
                            num, listPointer,
                            &event_);
                }
                else
                {
                    stcd = OpenCL.clEnqueueNDRangeKernel(
                            commandQueue.Pointer,
                            Pointer,
                            Dimension, null, WorkSizes, null,
                            num, listPointer,
                            &event_);
                }

                stcd.CheckError();
            }

            return new Event(event_);
        }
        ///<inheritdoc cref="NDRange(CommandQueue, Event[])"/>
        public Event NDRange(CommandQueue commandQueue, long[] workSizes)
        {
            SetWorkSize(workSizes);
            return NDRange(commandQueue);
        }
        ///<inheritdoc cref="NDRange(CommandQueue, Event[])"/>
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
            Marshal.FreeCoTaskMem((IntPtr)WorkSizes);
            Marshal.FreeCoTaskMem((IntPtr)LocalSizes);

            OpenCL.clReleaseKernel(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
