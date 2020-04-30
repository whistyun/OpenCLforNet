using NUnit.Framework;
using OpenCLforNet.PlatformLayer;
using OpenCLforNet.Runtime;
using System;

namespace OpenCLforNetTest
{
    public class TestBase
    {
        protected Device Device;
        protected Context Context;
        protected CommandQueue CommandQueue;

        [SetUp]
        public void Setup()
        {
            Device = new Device();
            Context = Device.CreateContext();
            CommandQueue = Context.CreateCommandQueue();
        }

        [TearDown]
        public void TearDown()
        {
            CommandQueue.Dispose();
            Context.Dispose();
        }
    }
}