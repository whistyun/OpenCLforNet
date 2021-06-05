using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Function;

namespace OpenCLforNet.Runtime
{
    public unsafe class CommandQueue : IDisposable
    {
        private bool isDisposed = false;

        public Context Context { get; }
        public Device Device { get; }
        public void* Pointer { get; }
        /// <summary>
        /// clCreateCommandQueue(context, device, CL_QUEUE_PROFILING_ENABLE)
        /// </summary>
        /// <param name="context">OpenCL context</param>
        /// <param name="device">Device to run jobs enqueued into this queue</param>
        public CommandQueue(Context context, Device device) : this(context, device, cl_command_queue_properties.CL_QUEUE_PROFILING_ENABLE) { }
        /// <summary>
        /// clCreateCommandQueue(context, device, options)
        /// </summary>
        /// <param name="context">OpenCL context</param>
        /// <param name="device">Device to run jobs enqueued into this queue</param>
        /// <param name="options"></param>
        public CommandQueue(Context context, Device device, cl_command_queue_properties options)
        {
            var status = cl_status_code.CL_SUCCESS;
            Context = context;
            Device = device;
            Pointer = OpenCL.clCreateCommandQueue(context.Pointer, device.Pointer, options, &status);
            status.CheckError();
        }

        ~CommandQueue() => Dispose(false);

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
        public Event NDRangeKernel(Kernel kernel, params Event[] eventWaitList)
        {
            return kernel.NDRange(commandQueue: this, eventWaitList: eventWaitList);
        }
        /// <summary>
        /// clFlush(this)
        /// </summary>
        public void Flush()
        {
            OpenCL.clFlush(Pointer).CheckError();
        }
        /// <summary>
        /// clFinish(this)
        /// </summary>
        public void WaitFinish()
        {
            OpenCL.clFinish(Pointer).CheckError();
        }

        protected void DisposeUnManaged()
        {
            OpenCL.clReleaseCommandQueue(Pointer).CheckError();
        }

        protected virtual void DisposeManaged() { }

        public virtual void Release() => Dispose();
    }
}
